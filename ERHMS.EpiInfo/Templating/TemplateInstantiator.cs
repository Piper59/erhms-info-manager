using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.EpiInfo.Infrastructure;
using ERHMS.EpiInfo.Templating.Xml;
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
            }
        }

        private const char RelateConditionSeparator = ':';
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
                InstantiateSourceTable(xTable);
            }
            ICollection<Field> fields = InstantiateCore();
            Progress?.Report("Postprocessing fields");
            foreach (Field field in fields)
            {
                PostprocessField(field);
            }
            Progress?.Report("Adding grid tables");
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                InstantiateGridTable(xTable);
            }
        }

        private void InstantiateSourceTable(XTable xTable)
        {
            Progress?.Report($"Adding source table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            if (context.TableNameGenerator.Exists(table.TableName))
            {
                if (table.DataEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    return;
                }
                string original = table.TableName;
                string modified = context.TableNameGenerator.MakeUnique(original);
                context.TableNameMap[original] = modified;
                table.TableName = modified;
                Progress?.Report($"Renamed source table: {table.TableName}");
            }
            string[] columnNames = table.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();
            Metadata.CreateCodeTable(table.TableName, columnNames);
            Metadata.SaveCodeTableData(table, table.TableName, columnNames);
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

        protected IEnumerable<Field> InstantiateFields(XView xView, View view)
        {
            context.SetView(view);
            foreach (XPage xPage in xView.XPages)
            {
                Page page = InstantiatePage(xPage, view);
                foreach (Field field in InstantiateFieldsInternal(xPage, page))
                {
                    yield return field;
                }
            }
        }

        protected IEnumerable<Field> InstantiateFields(XPage xPage, Page page)
        {
            context.SetView(page.GetView());
            return InstantiateFieldsInternal(xPage, page);
        }

        private IEnumerable<Field> InstantiateFieldsInternal(XPage xPage, Page page)
        {
            return xPage.XFields.Select(xField => InstantiateField(xField, page));
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
            MapSourceTableName(field as TableBasedDropDownField);
            MapRelatedViewId(field as RelatedViewField);
            field.SaveToDb();
            context.FieldIdMap[xField.FieldId] = field.Id;
            return field;
        }

        private void MapSourceTableName(TableBasedDropDownField field)
        {
            if (field == null)
            {
                return;
            }
            if (context.TableNameMap.TryGetValue(field.SourceTableName, out string tableName))
            {
                field.SourceTableName = tableName;
            }
        }

        private void MapRelatedViewId(RelatedViewField field)
        {
            if (field == null)
            {
                return;
            }
            field.RelatedViewID = context.ViewIdMap[field.RelatedViewID];
        }

        private void PostprocessField(Field field)
        {
            MapSourceFieldId(field as MirrorField);
            MapRelateConditions(field as DDLFieldOfCodes);
        }

        private void MapSourceFieldId(MirrorField field)
        {
            if (field == null)
            {
                return;
            }
            field.SourceFieldId = context.FieldIdMap[field.SourceFieldId];
            field.SaveToDb();
        }

        private void MapRelateConditions(DDLFieldOfCodes field)
        {
            if (field == null || string.IsNullOrEmpty(field.AssociatedFieldInformation))
            {
                return;
            }
            IEnumerable<string> conditions = field.AssociatedFieldInformation.Split(Constants.LIST_SEPARATOR);
            field.AssociatedFieldInformation = string.Join(
                Constants.LIST_SEPARATOR.ToString(),
                conditions.Select(condition => MapRelateCondition(condition)));
            field.SaveToDb();
        }

        private string MapRelateCondition(string condition)
        {
            IList<string> chunks = condition.Split(RelateConditionSeparator);
            if (chunks.Count != 2)
            {
                return condition;
            }
            string columnName = chunks[0];
            if (!int.TryParse(chunks[1], out int fieldId))
            {
                return condition;
            }
            if (!context.FieldIdMap.TryGetValue(fieldId, out fieldId))
            {
                return condition;
            }
            return columnName + RelateConditionSeparator + fieldId;
        }

        private void InstantiateGridTable(XTable xTable)
        {
            Progress?.Report($"Adding grid table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            foreach (DataRow row in table.Rows)
            {
                if (IgnoredGridColumnNames.Contains(row.Field<string>(ColumnNames.NAME)))
                {
                    continue;
                }
                int fieldId = row.Field<int>(ColumnNames.FIELD_ID);
                row.SetField(ColumnNames.FIELD_ID, context.FieldIdMap[fieldId]);
                Metadata.AddGridColumn(row);
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
            return InstantiateFields(xView, view).ToList();
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
            return InstantiateFields(xPage, page).ToList();
        }

        private void AddCheckCode(XView xView)
        {
            string checkCode = xView.CheckCode.Trim();
            if (!View.CheckCode.Contains(checkCode))
            {
                View.CheckCode = (View.CheckCode + "\n" + checkCode).Trim();
                Metadata.UpdateView(View);
            }
        }
    }
}
