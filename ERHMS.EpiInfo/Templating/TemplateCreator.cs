using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public abstract class TemplateCreator
    {
        protected class ContextImpl
        {
            public IMetadataProvider Metadata { get; }
            public XTemplate XTemplate { get; set; }

            private readonly ISet<string> sourceTableNames = new HashSet<string>(NameComparer.Default);
            public IEnumerable<string> SourceTableNames => sourceTableNames;

            private readonly ICollection<GridColumnDataTable> gridTables = new List<GridColumnDataTable>();
            public IEnumerable<GridColumnDataTable> GridTables => gridTables;

            public ContextImpl(IMetadataProvider metadata)
            {
                Metadata = metadata;
            }

            private bool AddSourceTableName(MetaFieldType fieldType, string tableName)
            {
                if (fieldType.IsTableBased() && !string.IsNullOrEmpty(tableName))
                {
                    return sourceTableNames.Add(tableName);
                }
                else
                {
                    return false;
                }
            }

            public void OnXFieldCreated(XField xField)
            {
                AddSourceTableName(xField.FieldType, xField.SourceTableName);
                if (xField.FieldType == MetaFieldType.Grid)
                {
                    DataTable gridColumnData = Metadata.GetGridColumns(xField.FieldId);
                    gridColumnData.TableName = xField.Name;
                    GridColumnDataTable gridTable = new GridColumnDataTable(gridColumnData);
                    gridTables.Add(gridTable);
                    foreach (GridColumnDataRow gridColumn in gridTable)
                    {
                        AddSourceTableName(gridColumn.FieldType, gridColumn.SourceTableName);
                    }
                }
            }
        }

        protected IMetadataProvider Metadata { get; }
        protected abstract string DisplayName { get; }
        public IProgress<string> Progress { get; set; }
        protected ContextImpl Context { get; private set; }

        protected TemplateCreator(IMetadataProvider metadata)
        {
            Metadata = metadata;
        }

        protected abstract XTemplate CreateCore();

        public XTemplate Create()
        {
            Context = new ContextImpl(Metadata);
            try
            {
                Context.XTemplate = CreateCore();
                CreateXSourceTables();
                CreateXGridTables();
                return Context.XTemplate;
            }
            finally
            {
                Context = null;
            }
        }

        protected XView CreateXView(XProject xProject, View view)
        {
            Progress?.Report($"Adding view: {view.Name}");
            XView xView = XView.Create(view);
            xProject.Add(xView);
            foreach (Page page in view.Pages)
            {
                CreateXPage(xView, page);
            }
            return xView;
        }

        protected XPage CreateXPage(XView xView, Page page)
        {
            Progress?.Report($"Adding page: {page.Name}");
            XPage xPage = XPage.Create(page);
            xView.Add(xPage);
            DataTable fieldData = Metadata.GetFieldsOnPageAsDataTable(page.Id);
            FieldDataTable fields = new FieldDataTable(fieldData);
            IComparer<FieldDataRow> fieldComparer = new FieldDataRowComparer.ByGroupHoistingTabOrder(fields);
            foreach (FieldDataRow field in fields.OrderBy(field => field, fieldComparer))
            {
                CreateXField(xPage, field);
            }
            return xPage;
        }

        private XField CreateXField(XPage xPage, FieldDataRow field)
        {
            Progress?.Report($"Adding field: {field.Name}");
            XField xField = XField.Create(field);
            xPage.Add(xField);
            Context.OnXFieldCreated(xField);
            return xField;
        }

        private void CreateXSourceTables()
        {
            foreach (string tableName in Context.SourceTableNames.OrderBy(tableName => tableName, NameComparer.Default))
            {
                CreateXSourceTable(tableName);
            }
        }

        private XTable CreateXSourceTable(string tableName)
        {
            Progress?.Report($"Adding source table: {tableName}");
            DataTable table = Metadata.GetCodeTableData(tableName);
            XTable xTable = XTable.Create(ElementNames.SourceTable, table);
            Context.XTemplate.Add(xTable);
            return xTable;
        }

        private void CreateXGridTables()
        {
            foreach (GridColumnDataTable gridTable in Context.GridTables)
            {
                CreateXGridTable(gridTable);
            }
        }

        private XTable CreateXGridTable(GridColumnDataTable gridTable)
        {
            Progress?.Report($"Adding grid table: {gridTable.Table.TableName}");
            XTable xTable = XTable.Create(ElementNames.GridTable, gridTable);
            Context.XTemplate.Add(xTable);
            return xTable;
        }
    }
}
