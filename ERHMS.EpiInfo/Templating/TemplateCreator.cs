using Epi;
using Epi.Data.Services;
using ERHMS.Data;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public abstract class TemplateCreator
    {
        protected IMetadataProvider Metadata { get; }
        protected abstract string DisplayName { get; }
        public IProgress<string> Progress { get; set; }

        protected TemplateCreator(IMetadataProvider metadata)
        {
            Metadata = metadata;
        }

        protected abstract XTemplate CreateCore();

        public XTemplate Create()
        {
            Progress?.Report($"Creating Epi Info template: {DisplayName}");
            XTemplate xTemplate = CreateCore();
            AddSourceTables(xTemplate);
            AddGridTables(xTemplate);
            return xTemplate;
        }

        protected void AddView(View view, XProject xProject)
        {
            Progress?.Report($"Adding view: {view.Name}");
            XView xView = XView.Create(view);
            xProject.Add(xView);
            foreach (Page page in view.Pages)
            {
                AddPage(page, xView);
            }
        }

        private IEnumerable<FieldDataRow> GetFields(int pageId)
        {
            DataTable table = Metadata.GetFieldsOnPageAsDataTable(pageId);
            table.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            FieldDataTable fields = new FieldDataTable(table);
            IDictionary<string, FieldDataRow> fieldsByName = fields.ToDictionary(field => field.Name, StringComparer.OrdinalIgnoreCase);
            foreach (FieldDataRow field in fields.Where(field => field.FieldType == MetaFieldType.Group))
            {
                double? minTabIndex = field.ListItems
                    .Where(fieldName => fieldsByName.ContainsKey(fieldName))
                    .Select(fieldName => fieldsByName[fieldName].TabIndex)
                    .DefaultIfEmpty()
                    .Min();
                if (minTabIndex != null)
                {
                    field.TabIndex = minTabIndex - 0.5;
                }
            }
            return fields.OrderBy(field => field.TabIndex).ThenBy(field => field.FieldId);
        }

        protected void AddPage(Page page, XView xView)
        {
            View view = page.GetView();
            Progress?.Report($"Adding page: {view.Name}/{page.Name}");
            XPage xPage = XPage.Create(page);
            xView.Add(xPage);
            foreach (FieldDataRow field in GetFields(page.Id))
            {
                Progress?.Report($"Adding field: {view.Name}/{page.Name}/{field.Name}");
                xPage.Add(XField.Create(field));
            }
        }

        private void AddSourceTables(XTemplate xTemplate)
        {
            Progress?.Report("Adding source tables");
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in xTemplate.XProject.XFields)
            {
                if (xField.FieldType.IsTableBased() && !string.IsNullOrEmpty(xField.SourceTableName))
                {
                    tableNames.Add(xField.SourceTableName);
                }
                else if (xField.FieldType == MetaFieldType.Grid)
                {
                    DataTable table = Metadata.GetGridColumns(xField.FieldId);
                    GridColumnDataTable gridColumns = new GridColumnDataTable(table);
                    foreach (GridColumnDataRow gridColumn in gridColumns)
                    {
                        if (gridColumn.FieldType.IsTableBased() && !string.IsNullOrEmpty(gridColumn.SourceTableName))
                        {
                            tableNames.Add(gridColumn.SourceTableName);
                        }
                    }
                }
            }
            foreach (string tableName in tableNames.OrderBy(tableName => tableName, StringComparer.OrdinalIgnoreCase))
            {
                AddSourceTable(xTemplate, tableName);
            }
        }

        private void AddSourceTable(XTemplate xTemplate, string tableName)
        {
            Progress?.Report($"Adding source table: {tableName}");
            DataTable table = Metadata.GetCodeTableData(tableName);
            table.TableName = tableName;
            xTemplate.Add(XTable.Create(ElementNames.SourceTable, table));
        }

        private void AddGridTables(XTemplate xTemplate)
        {
            Progress?.Report("Adding grid tables");
            foreach (XField xField in xTemplate.XProject.XFields.Where(xField => xField.FieldType == MetaFieldType.Grid))
            {
                AddGridTable(xTemplate, xField);
            }
        }

        private void AddGridTable(XTemplate xTemplate, XField xField)
        {
            Progress?.Report($"Adding grid table: {xField.Name}");
            DataTable table = Metadata.GetGridColumns(xField.FieldId);
            table.TableName = xField.Name;
            xTemplate.Add(XTable.Create(ElementNames.GridTable, table));
        }
    }

    public class ProjectTemplateCreator : TemplateCreator
    {
        public Project Project { get; }
        protected override string DisplayName => Project.DisplayName;

        public ProjectTemplateCreator(Project project)
            : base(project.Metadata)
        {
            Project = project;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.Project);
            xTemplate.Name = Project.Name;
            xTemplate.Description = Project.Description;
            XProject xProject = XProject.Create(Project);
            xTemplate.Add(xProject);
            foreach (View view in Project.Views)
            {
                AddView(view, xProject);
            }
            return xTemplate;
        }
    }

    public class ViewTemplateCreator : TemplateCreator
    {
        public View View { get; }
        protected override string DisplayName => View.DisplayName;

        public ViewTemplateCreator(View view)
            : base(view.GetMetadata())
        {
            View = view;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.View);
            xTemplate.Name = View.Name;
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            AddView(View, xProject);
            xProject.RemoveRelationships();
            return xTemplate;
        }
    }

    public class PageTemplateCreator : TemplateCreator
    {
        public Page Page { get; }
        protected override string DisplayName => Page.DisplayName;

        public PageTemplateCreator(Page page)
            : base(page.GetMetadata())
        {
            Page = page;
        }

        protected override XTemplate CreateCore()
        {
            XTemplate xTemplate = XTemplate.Create(TemplateLevel.Page);
            xTemplate.Name = Page.Name;
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            XView xView = new XView
            {
                CheckCode = Page.GetView().CheckCode
            };
            xProject.Add(xView);
            AddPage(Page, xView);
            xProject.RemoveRelationships();
            return xTemplate;
        }
    }
}
