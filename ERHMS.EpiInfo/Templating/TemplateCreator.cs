using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public abstract class TemplateCreator
    {
        protected class ContextImpl
        {
            public MetadataDbProvider Metadata { get; }
            public XTemplate XTemplate { get; set; }

            private readonly ISet<string> sourceTableNames = new SortedSet<string>(NameComparer.Default);
            public IEnumerable<string> SourceTableNames => sourceTableNames;

            private readonly ICollection<GridColumnDataTable> gridTables = new List<GridColumnDataTable>();
            public IEnumerable<GridColumnDataTable> GridTables => gridTables;

            public ContextImpl(MetadataDbProvider metadata)
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
                    GridColumnDataTable gridColumns = Metadata.GetGridColumnDataTable(xField.FieldId);
                    gridColumns.Table.TableName = xField.Name;
                    gridTables.Add(gridColumns);
                    foreach (GridColumnDataRow gridColumn in gridColumns)
                    {
                        AddSourceTableName(gridColumn.FieldType, gridColumn.SourceTableName);
                    }
                }
            }
        }

        protected MetadataDbProvider Metadata { get; }
        public IProgress<string> Progress { get; set; }
        protected ContextImpl Context { get; private set; }

        protected TemplateCreator(IMetadataProvider metadata)
        {
            Metadata = (MetadataDbProvider)metadata;
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
            FieldDataTable fields = Metadata.GetFieldDataTableForPage(page.Id);
            IComparer<FieldDataRow> comparer = new FieldDataRowComparer.ByEffectiveTabIndex(fields);
            foreach (FieldDataRow field in fields.OrderBy(field => field, comparer))
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
            foreach (string tableName in Context.SourceTableNames)
            {
                Progress?.Report($"Adding source table: {tableName}");
                DataTable table = Metadata.GetCodeTableData(tableName);
                table.TableName = tableName;
                CreateXTable(ElementNames.SourceTable, table);
            }
        }

        private void CreateXGridTables()
        {
            foreach (GridColumnDataTable gridColumns in Context.GridTables)
            {
                Progress?.Report($"Adding grid table: {gridColumns.Table.TableName}");
                CreateXTable(ElementNames.GridTable, gridColumns);
            }
        }

        private XTable CreateXTable(XName name, DataTable table)
        {
            XTable xTable = XTable.Create(name, table);
            foreach (DataRow item in table.Rows)
            {
                xTable.Add(XItem.Create(item));
            }
            Context.XTemplate.Add(xTable);
            return xTable;
        }
    }
}
