using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo.Infrastructure;
using ERHMS.EpiInfo.Templates.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Templates
{
    public abstract class TemplateCreator
    {
        private const double GroupTabIndexDifference = 0.1;

        private static IEnumerable<DataRow> GetFields(Page page)
        {
            DataTable table = page.GetMetadata().GetFieldsOnPageAsDataTable(page.Id);
            table.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            IDictionary<string, DataRow> fields = table.Rows.Cast<DataRow>()
                .ToDictionary(field => field.Field<string>(ColumnNames.NAME), StringComparer.OrdinalIgnoreCase);
            IEnumerable<DataRow> groups = fields.Values
                .Where(field => (MetaFieldType)field.Field<int>(ColumnNames.FIELD_TYPE_ID) == MetaFieldType.Group);
            foreach (DataRow group in groups)
            {
                IEnumerable<DataRow> members = group.Field<string>(ColumnNames.LIST)
                    ?.Split(Constants.LIST_SEPARATOR)
                    ?.Select(fieldName => fields.TryGetValue(fieldName, out DataRow field) ? field : null);
                double? tabIndexMin = members?.Min(member => member?.Field<double?>(ColumnNames.TAB_INDEX));
                if (tabIndexMin == null)
                {
                    continue;
                }
                group.SetField(ColumnNames.TAB_INDEX, tabIndexMin.Value - GroupTabIndexDifference);
            }
            return fields.Values.OrderBy(field => field.Field<double?>(ColumnNames.TAB_INDEX));
        }

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
            xTemplate.OnCreated();
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
            foreach (DataRow field in GetFields(page))
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
                    DataTable gridColumns = Metadata.GetGridColumns(xField.FieldId);
                    foreach (DataRow gridColumn in gridColumns.Rows)
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
                if (string.IsNullOrEmpty(tableName))
                {
                    continue;
                }
                Progress?.Report($"Adding source table: {tableName}");
                DataTable table = Metadata.GetCodeTableData(tableName);
                table.TableName = tableName;
                xTemplate.Add(XTable.Create(ElementNames.SourceTable, table));
            }
        }

        private void AddGridTables(XTemplate xTemplate)
        {
            Progress?.Report("Adding grid tables");
            foreach (XField xField in xTemplate.XProject.XFields)
            {
                if (xField.FieldType != MetaFieldType.Grid)
                {
                    continue;
                }
                Progress?.Report($"Adding grid table: {xField.Name}");
                DataTable table = Metadata.GetGridColumns(xField.FieldId);
                table.TableName = xField.Name;
                xTemplate.Add(XTable.Create(ElementNames.GridTable, table));
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
            XTemplate xTemplate = new XTemplate(TemplateLevel.Project)
            {
                Name = Project.Name,
                Description = Project.Description
            };
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
            XTemplate xTemplate = new XTemplate(TemplateLevel.View)
            {
                Name = View.Name
            };
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            AddView(View, xProject);
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
            XTemplate xTemplate = new XTemplate(TemplateLevel.Page)
            {
                Name = Page.Name
            };
            XProject xProject = new XProject();
            xTemplate.Add(xProject);
            XView xView = new XView
            {
                CheckCode = Page.GetView().CheckCode
            };
            xProject.Add(xView);
            AddPage(Page, xView);
            return xTemplate;
        }
    }
}
