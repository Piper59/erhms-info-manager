using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common.Logging;
using ERHMS.Data;
using ERHMS.EpiInfo.Metadata;
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
        protected class Context
        {
            public TableNameGenerator TableNameGenerator { get; }
            public ViewNameGenerator ViewNameGenerator { get; }
            public PageNameGenerator PageNameGenerator { get; private set; }
            public FieldNameGenerator FieldNameGenerator { get; private set; }
            public IDictionary<string, string> TableNameMap { get; } = new Dictionary<string, string>();
            public IDictionary<string, string> FieldNameMap { get; } = new Dictionary<string, string>();
            public IDictionary<int, int> ViewIdMap { get; } = new Dictionary<int, int>();
            public IDictionary<int, int> FieldIdMap { get; } = new Dictionary<int, int>();

            public Context(Project project)
            {
                TableNameGenerator = new TableNameGenerator(project);
                ViewNameGenerator = new ViewNameGenerator(project);
            }

            public void SetView(View view)
            {
                PageNameGenerator = new PageNameGenerator(view);
                FieldNameGenerator = new FieldNameGenerator(view);
                FieldNameMap.Clear();
            }
        }

        protected Context context;

        public abstract TemplateLevel Level { get; }
        public XTemplate XTemplate { get; }
        protected IMetadataProvider Metadata { get; }
        public IProgress<string> Progress { get; set; }

        protected TemplateInstantiator(XTemplate xTemplate, IMetadataProvider metadata)
        {
            if (xTemplate.Level != Level)
            {
                string message = $"Template level '{xTemplate.Level}' does not match instantiator level '{Level}'.";
                throw new ArgumentException(message, nameof(xTemplate));
            }
            XTemplate = xTemplate;
            Metadata = metadata;
        }

        protected abstract void InstantiateCore();

        public void Instantiate()
        {
            Progress?.Report($"Instantiating Epi Info template: {XTemplate.Name}");
            context = new Context(Metadata.Project);
            InstantiateSourceTables();
            InstantiateCore();
            InstantiateGridTables();
        }

        private void InstantiateSourceTables()
        {
            Progress?.Report("Adding source tables");
            foreach (XTable xTable in XTemplate.XSourceTables)
            {
                InstantiateSourceTable(xTable);
            }
        }

        private void InstantiateSourceTable(XTable xTable)
        {
            Progress?.Report($"Adding source table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            bool exists = false;
            if (context.TableNameGenerator.Exists(table.TableName))
            {
                if (table.DataEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    exists = true;
                }
                else
                {
                    table.TableName = context.TableNameGenerator.Generate(table.TableName);
                    Progress?.Report($"Renamed source table: {table.TableName}");
                }
            }
            if (!exists)
            {
                string[] columnNames = table.Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();
                Metadata.CreateCodeTable(table.TableName, columnNames);
                Metadata.SaveCodeTableData(table, table.TableName, columnNames);
                context.TableNameGenerator.Add(table.TableName);
            }
            context.TableNameMap[xTable.TableName] = table.TableName;
        }

        protected View InstantiateView(XView xView, Project project)
        {
            Progress?.Report($"Adding view: {xView.Name}");
            View view = xView.Instantiate(project);
            if (context.ViewNameGenerator.Exists(view.Name))
            {
                view.Name = context.ViewNameGenerator.Generate(view.Name);
                Progress?.Report($"Renamed view: {view.Name}");
            }
            Metadata.InsertView(view);
            project.Views.Add(view);
            context.ViewNameGenerator.Add(view.Name);
            context.ViewIdMap[xView.ViewId] = view.Id;
            return view;
        }

        protected Page InstantiatePage(XPage xPage, View view)
        {
            Progress?.Report($"Adding page: {view.Name}/{xPage.Name}");
            Page page = xPage.Instantiate(view);
            if (context.PageNameGenerator.Exists(page.Name))
            {
                page.Name = context.PageNameGenerator.Generate(page.Name);
                Progress?.Report($"Renamed page: {page.Name}");
            }
            page.Position = view.Pages.Count;
            Metadata.InsertPage(page);
            view.Pages.Add(page);
            context.PageNameGenerator.Add(page.Name);
            return page;
        }

        protected Field InstantiateField(XField xField, Page page)
        {
            View view = page.GetView();
            Progress?.Report($"Adding field: {view.Name}/{page.Name}/{xField.Name}");
            Field field = xField.Instantiate(page);
            if (context.FieldNameGenerator.Exists(field.Name))
            {
                field.Name = context.FieldNameGenerator.Generate(field.Name);
                Progress?.Report($"Renamed field: {field.Name}");
            }
            OnFieldInstantiating(field);
            field.SaveToDb();
            context.FieldNameGenerator.Add(field.Name);
            context.FieldNameMap[xField.Name] = field.Name;
            context.FieldIdMap[xField.FieldId] = field.Id;
            return field;
        }

        private void OnFieldInstantiating(Field field)
        {
            if (field is RelatedViewField relatedViewField)
            {
                if (context.ViewIdMap.TryGetValue(relatedViewField.RelatedViewID, out int mappedRelatedViewId))
                {
                    relatedViewField.RelatedViewID = mappedRelatedViewId;
                }
                else
                {
                    Log.Instance.Warn($"Related view not found: {relatedViewField.RelatedViewID}");
                }
            }
            else if (field is TableBasedDropDownField tableBasedDropDownField)
            {
                if (context.TableNameMap.TryGetValue(tableBasedDropDownField.SourceTableName, out string mappedSourceTableName))
                {
                    tableBasedDropDownField.SourceTableName = mappedSourceTableName;
                }
                else
                {
                    Log.Instance.Warn($"Source table not found: {tableBasedDropDownField.SourceTableName}");
                }
            }
        }

        protected void OnViewFieldsInstantiated(IEnumerable<Field> fields)
        {
            foreach (Field field in fields)
            {
                if (field is GroupField groupField)
                {
                    groupField.ChildFieldNames = GroupFieldMapper.MapChildFieldNames(
                        groupField.ChildFieldNames,
                        context.FieldNameMap.TryGetValue);
                    groupField.SaveToDb();
                }
                else if (field is MirrorField mirrorField)
                {
                    if (context.FieldIdMap.TryGetValue(mirrorField.SourceFieldId, out int mappedSourceFieldId))
                    {
                        mirrorField.SourceFieldId = mappedSourceFieldId;
                        mirrorField.SaveToDb();
                    }
                    else
                    {
                        Log.Instance.Warn($"Source field not found: {mirrorField.SourceFieldId}");
                    }
                }
            }
        }

        protected void OnProjectFieldsInstantiated(IEnumerable<Field> fields)
        {
            foreach (Field field in fields)
            {
                if (field is DDLFieldOfCodes ddlFieldOfCodes)
                {
                    ddlFieldOfCodes.AssociatedFieldInformation = DDLFieldOfCodesMapper.MapAssociatedFieldInformation(
                        ddlFieldOfCodes.AssociatedFieldInformation,
                        context.FieldIdMap.TryGetValue);
                    ddlFieldOfCodes.SaveToDb();
                }
            }
        }

        private void InstantiateGridTables()
        {
            Progress?.Report("Adding grid tables");
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                InstantiateGridTable(xTable);
            }
        }

        private void InstantiateGridTable(XTable xTable)
        {
            Progress?.Report($"Adding grid table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            GridColumnDataTable gridColumns = new GridColumnDataTable(table);
            foreach (GridColumnDataRow gridColumn in gridColumns.Where(gridColumn => !gridColumn.IsMetadata()))
            {
                OnGridColumnInstantiating(gridColumn);
                Metadata.AddGridColumn(gridColumn);
            }
        }

        private void OnGridColumnInstantiating(GridColumnDataRow gridColumn)
        {
            if (context.FieldIdMap.TryGetValue(gridColumn.FieldId, out int mappedFieldId))
            {
                gridColumn.FieldId = mappedFieldId;
            }
            else
            {
                Log.Instance.Warn($"Field not found: {gridColumn.FieldId}");
            }
            if (context.TableNameMap.TryGetValue(gridColumn.SourceTableName, out string mappedSourceTableName))
            {
                gridColumn.SourceTableName = mappedSourceTableName;
            }
            else
            {
                Log.Instance.Warn($"Source table not found: {gridColumn.SourceTableName}");
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

        protected override void InstantiateCore()
        {
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                InstantiateView(xView, Project);
            }
            ICollection<Field> projectFields = new List<Field>();
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                int mappedViewId = context.ViewIdMap[xView.ViewId];
                View view = Project.Views.GetViewById(mappedViewId);
                context.SetView(view);
                ICollection<Field> viewFields = new List<Field>();
                foreach (XPage xPage in xView.XPages)
                {
                    Page page = InstantiatePage(xPage, view);
                    foreach (XField xField in xPage.XFields)
                    {
                        Field field = InstantiateField(xField, page);
                        viewFields.Add(field);
                        projectFields.Add(field);
                    }
                }
                OnViewFieldsInstantiated(viewFields);
            }
            OnProjectFieldsInstantiated(projectFields);
        }
    }

    public class ViewTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.View;
        public Project Project { get; }
        public View View { get; private set; }

        public ViewTemplateInstantiator(XTemplate xTemplate, Project project)
            : base(xTemplate, project.Metadata)
        {
            Project = project;
        }

        protected override void InstantiateCore()
        {
            XView xView = XTemplate.XProject.XView;
            View = InstantiateView(xView, Project);
            context.SetView(View);
            ICollection<Field> fields = new List<Field>();
            foreach (XPage xPage in xView.XPages)
            {
                Page page = InstantiatePage(xPage, View);
                foreach (XField xField in xPage.XFields)
                {
                    Field field = InstantiateField(xField, page);
                    fields.Add(field);
                }
            }
            OnViewFieldsInstantiated(fields);
            OnProjectFieldsInstantiated(fields);
        }
    }

    public class PageTemplateInstantiator : TemplateInstantiator
    {
        public override TemplateLevel Level => TemplateLevel.Page;
        public View View { get; }
        public Page Page { get; private set; }

        public PageTemplateInstantiator(XTemplate xTemplate, View view)
            : base(xTemplate, view.GetMetadata())
        {
            View = view;
        }

        protected override void InstantiateCore()
        {
            context.SetView(View);
            XView xView = XTemplate.XProject.XView;
            string checkCode = xView.CheckCode.Trim();
            if (!View.CheckCode.Contains(checkCode))
            {
                View.CheckCode = $"{View.CheckCode}\n{checkCode}".Trim();
                Metadata.UpdateView(View);
            }
            XPage xPage = xView.XPages.Single();
            ICollection<Field> fields = new List<Field>();
            foreach (XField xField in xPage.XFields)
            {
                Field field = InstantiateField(xField, Page);
                fields.Add(field);
            }
            OnViewFieldsInstantiated(fields);
            OnProjectFieldsInstantiated(fields);
        }
    }
}
