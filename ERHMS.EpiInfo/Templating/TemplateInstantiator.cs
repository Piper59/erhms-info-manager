using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.EpiInfo.Templating.Xml.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public abstract class TemplateInstantiator
    {
        private class Context
        {
            public TableNameGenerator TableNameGenerator { get; }
            public ViewNameGenerator ViewNameGenerator { get; }
            public PageNameGenerator PageNameGenerator { get; private set; }
            public FieldNameGenerator FieldNameGenerator { get; private set; }
            public IDictionary<string, string> TableNameMap { get; }
            public IDictionary<int, int> ViewIdMap { get; }
            public IDictionary<int, int> FieldIdMap { get; }
            public IDictionary<string, string> FieldNameMap { get; private set; }

            public Context(Project project)
            {
                TableNameGenerator = new TableNameGenerator(project);
                ViewNameGenerator = new ViewNameGenerator(project);
                TableNameMap = new Dictionary<string, string>();
                ViewIdMap = new Dictionary<int, int>();
                FieldIdMap = new Dictionary<int, int>();
            }

            public void SetView(View view)
            {
                PageNameGenerator = new PageNameGenerator(view);
                FieldNameGenerator = new FieldNameGenerator(view);
                FieldNameMap = new Dictionary<string, string>();
            }
        }

        private static readonly ISet<string> IgnoredGridColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ColumnNames.UNIQUE_ROW_ID,
            ColumnNames.REC_STATUS,
            ColumnNames.FOREIGN_KEY,
            ColumnNames.GLOBAL_RECORD_ID
        };

        private Context context;

        public abstract TemplateLevel Level { get; }
        public XTemplate XTemplate { get; }
        protected IMetadataProvider Metadata { get; }
        public IProgress<string> Progress { get; set; }

        protected TemplateInstantiator(XTemplate xTemplate, IMetadataProvider metadata)
        {
            if (xTemplate.Level != Level)
            {
                throw new ArgumentException($"Template level must be '{Level}'.");
            }
            XTemplate = xTemplate;
            Metadata = metadata;
        }

        protected abstract ICollection<Field> InstantiateCore();

        public void Instantiate()
        {
            Progress?.Report($"Instantiating template: {XTemplate.Name}");
            context = new Context(Metadata.Project);
            Progress?.Report("Adding source tables");
            foreach (XTable xTable in XTemplate.XSourceTables)
            {
                DataTable table = InstantiateSourceTable(xTable);
                context.TableNameMap[xTable.TableName] = table.TableName;
            }
            ICollection<Field> fields = InstantiateCore();
            OnProjectInstantiated(fields);
            Progress?.Report("Adding grid tables");
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                InstantiateGridTable(xTable);
            }
        }

        private DataTable InstantiateSourceTable(XTable xTable)
        {
            Progress?.Report($"Adding source table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            if (context.TableNameGenerator.Exists(table.TableName))
            {
                if (table.DataEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    return table;
                }
                table.TableName = context.TableNameGenerator.MakeUnique(table.TableName);
                Progress?.Report($"Renamed source table: {table.TableName}");
            }
            string[] columnNames = table.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();
            Metadata.CreateCodeTable(table.TableName, columnNames);
            Metadata.SaveCodeTableData(table, table.TableName, columnNames);
            return table;
        }

        protected View InstantiateView(XView xView, Project project)
        {
            Progress?.Report($"Adding view: {xView.Name}");
            View view = xView.Instantiate(project);
            if (context.ViewNameGenerator.Exists(view.Name) || context.ViewNameGenerator.Conflicts(view.Name))
            {
                view.Name = context.ViewNameGenerator.MakeUnique(view.Name);
                Progress?.Report($"Renamed view: {view.Name}");
            }
            Metadata.InsertView(view);
            project.Views.Add(view);
            context.ViewIdMap[xView.ViewId] = view.Id;
            return view;
        }

        protected Page InstantiatePage(XPage xPage, View view)
        {
            Progress?.Report($"Adding page: {view.Name}/{xPage.Name}");
            Page page = xPage.Instantiate(view);
            if (context.PageNameGenerator.Exists(page.Name))
            {
                page.Name = context.PageNameGenerator.MakeUnique(page.Name);
                Progress?.Report($"Renamed page: {page.Name}");
            }
            page.Position = view.Pages.Count;
            Metadata.InsertPage(page);
            view.Pages.Add(page);
            return page;
        }

        protected ICollection<Field> InstantiateFields(XView xView, View view)
        {
            context.SetView(view);
            ICollection<Field> fields = new List<Field>();
            foreach (XPage xPage in xView.XPages)
            {
                Page page = InstantiatePage(xPage, view);
                foreach (XField xField in xPage.XFields)
                {
                    fields.Add(InstantiateField(xField, page));
                }
            }
            OnViewInstantiated(fields);
            return fields;
        }

        protected ICollection<Field> InstantiateFields(XPage xPage, Page page)
        {
            context.SetView(page.GetView());
            ICollection<Field> fields = new List<Field>();
            foreach (XField xField in xPage.XFields)
            {
                fields.Add(InstantiateField(xField, page));
            }
            OnViewInstantiated(fields);
            return fields;
        }

        private Field InstantiateField(XField xField, Page page)
        {
            View view = page.GetView();
            Progress?.Report($"Adding field: {view.Name}/{page.Name}/{xField.Name}");
            Field field = xField.Instantiate(page);
            if (context.FieldNameGenerator.Exists(field.Name))
            {
                field.Name = context.FieldNameGenerator.MakeUnique(field.Name);
                Progress?.Report($"Renamed field: {field.Name}");
            }
            OnFieldInstantiating(field);
            field.SaveToDb();
            context.FieldIdMap[xField.FieldId] = field.Id;
            context.FieldNameMap[xField.Name] = field.Name;
            return field;
        }

        private void OnFieldInstantiating(Field field)
        {
            if (field is TableBasedDropDownField tbddf)
            {
                tbddf.SourceTableName = context.TableNameMap[tbddf.SourceTableName];
            }
            if (field is RelatedViewField rvf)
            {
                rvf.RelatedViewID = context.ViewIdMap[rvf.RelatedViewID];
            }
        }

        private void OnViewInstantiated(IEnumerable<Field> fields)
        {
            foreach (Field field in fields)
            {
                if (field is GroupField gf)
                {
                    string fieldNames = gf.ChildFieldNames;
                    if (!string.IsNullOrEmpty(fieldNames))
                    {
                        fieldNames = GroupFieldMapper.MapChildFieldNames(fieldNames, context.FieldNameMap);
                        gf.ChildFieldNames = fieldNames;
                        gf.SaveToDb();
                    }
                }
            }
        }

        private void OnProjectInstantiated(IEnumerable<Field> fields)
        {
            foreach (Field field in fields)
            {
                if (field is MirrorField mf)
                {
                    mf.SourceFieldId = context.FieldIdMap[mf.SourceFieldId];
                    mf.SaveToDb();
                }
                if (field is DDLFieldOfCodes ddlfoc)
                {
                    string conditions = ddlfoc.AssociatedFieldInformation;
                    if (!string.IsNullOrEmpty(conditions))
                    {
                        conditions = DDLFieldOfCodesMapper.MapRelateConditions(conditions, context.FieldIdMap);
                        ddlfoc.AssociatedFieldInformation = conditions;
                        ddlfoc.SaveToDb();
                    }
                }
            }
        }

        private void InstantiateGridTable(XTable xTable)
        {
            Progress?.Report($"Adding grid table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            foreach (DataRow item in table.Rows)
            {
                if (IgnoredGridColumnNames.Contains(item.Field<string>(ColumnNames.NAME)))
                {
                    continue;
                }
                int fieldId = item.Field<int>(ColumnNames.FIELD_ID);
                item.SetField(ColumnNames.FIELD_ID, context.FieldIdMap[fieldId]);
                item.SetField(
                    ColumnNames.SOURCE_TABLE_NAME,
                    context.TableNameMap[item.Field<string>(ColumnNames.SOURCE_TABLE_NAME)]);
                Metadata.AddGridColumn(item);
            }
        }
    }

    public class ProjectTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.Project;
        public Project Project { get; }

        public ProjectTemplateInstantiator(XTemplate xTemplate, Project project)
            : base(xTemplate, project.Metadata)
        {
            Project = project;
        }

        protected override ICollection<Field> InstantiateCore()
        {
            List<Field> fields = new List<Field>();
            ICollection<(XView, View)> views = new List<(XView, View)>();
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                View view = InstantiateView(xView, Project);
                views.Add((xView, view));
            }
            foreach ((XView xView, View view) in views)
            {
                fields.AddRange(InstantiateFields(xView, view));
            }
            return fields;
        }
    }

    public class ViewTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.View;
        public Project Project { get; }

        public ViewTemplateInstantiator(XTemplate xTemplate, Project project)
            : base(xTemplate, project.Metadata)
        {
            Project = project;
        }

        protected override ICollection<Field> InstantiateCore()
        {
            XView xView = XTemplate.XProject.XViews.Single();
            View view = InstantiateView(xView, Project);
            return InstantiateFields(xView, view);
        }
    }

    public class PageTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.Page;
        public View View { get; }

        public PageTemplateInstantiator(XTemplate xTemplate, View view)
            : base(xTemplate, view.GetMetadata())
        {
            View = view;
        }

        protected override ICollection<Field> InstantiateCore()
        {
            XView xView = XTemplate.XProject.XViews.Single();
            AddCheckCode(xView);
            XPage xPage = xView.XPages.Single();
            Page page = InstantiatePage(xPage, View);
            return InstantiateFields(xPage, page);
        }

        private void AddCheckCode(XView xView)
        {
            string checkCode = xView.CheckCode.Trim();
            if (!View.CheckCode.Contains(checkCode))
            {
                View.CheckCode = string.Concat(View.CheckCode, "\n", checkCode).Trim();
                Metadata.UpdateView(View);
            }
        }
    }
}
