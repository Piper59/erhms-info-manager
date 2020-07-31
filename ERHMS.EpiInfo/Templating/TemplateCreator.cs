using Epi;
using Epi.Data.Services;
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
            Progress?.Report($"Creating template: {DisplayName}");
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

        protected void AddPage(Page page, XView xView)
        {
            View view = page.GetView();
            Progress?.Report($"Adding page: {view.Name}/{page.Name}");
            XPage xPage = XPage.Create(page);
            xView.Add(xPage);
            foreach (DataRow field in Metadata.GetSortedFields(page.Id))
            {
                string fieldName = field.Field<string>(ColumnNames.NAME);
                Progress?.Report($"Adding field: {view.Name}/{page.Name}/{fieldName}");
                xPage.Add(XField.Create(field));
            }
        }

        private void AddSourceTables(XTemplate xTemplate)
        {
            Progress?.Report("Adding source tables");
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in xTemplate.XProject.XFields)
            {
                if (xField.FieldType.IsTableBased())
                {
                    tableNames.Add(xField.SourceTableName);
                }
                else if (xField.FieldType == MetaFieldType.Grid)
                {
                    foreach (DataRow gridColumn in Metadata.GetGridColumns(xField.FieldId).Rows)
                    {
                        if (gridColumn.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID).IsTableBased())
                        {
                            tableNames.Add(gridColumn.Field<string>(ColumnNames.SOURCE_TABLE_NAME));
                        }
                    }
                }
            }
            foreach (string tableName in tableNames.OrderBy(tableName => tableName, StringComparer.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    Progress?.Report($"Adding source table: {tableName}");
                    DataTable table = Metadata.GetCodeTableData(tableName);
                    table.TableName = tableName;
                    xTemplate.Add(XTable.Create(ElementNames.SourceTable, table));
                }
            }
        }

        private void AddGridTables(XTemplate xTemplate)
        {
            Progress?.Report("Adding grid tables");
            foreach (XField xField in xTemplate.XProject.XFields)
            {
                if (xField.FieldType == MetaFieldType.Grid)
                {
                    Progress?.Report($"Adding grid table: {xField.Name}");
                    DataTable table = Metadata.GetGridColumns(xField.FieldId);
                    table.TableName = xField.Name;
                    xTemplate.Add(XTable.Create(ElementNames.GridTable, table));
                }
            }
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
            xProject.RemoveRelateFields();
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
            xProject.RemoveRelateFields();
            return xTemplate;
        }
    }
}
