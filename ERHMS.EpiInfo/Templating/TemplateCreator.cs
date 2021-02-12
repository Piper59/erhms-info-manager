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
        protected class ContextImpl : IDisposable
        {
            public TemplateCreator Owner { get; }
            public XTemplate XTemplate { get; set; }

            private readonly ISet<string> sourceTableNames = new HashSet<string>(NameComparer.Default);
            public IEnumerable<string> SourceTableNames => sourceTableNames
                .OrderBy(tableName => tableName, NameComparer.Default);

            private readonly ICollection<GridColumnDataTable> gridTables = new List<GridColumnDataTable>();
            public IEnumerable<GridColumnDataTable> GridTables => gridTables;

            public ContextImpl(TemplateCreator owner)
            {
                Owner = owner;
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
                    DataTable gridTableData = Owner.Metadata.GetGridColumns(xField.FieldId);
                    gridTableData.TableName = xField.Name;
                    GridColumnDataTable gridTable = new GridColumnDataTable(gridTableData);
                    gridTables.Add(gridTable);
                    foreach (GridColumnDataRow gridColumn in gridTable)
                    {
                        AddSourceTableName(gridColumn.FieldType, gridColumn.SourceTableName);
                    }
                }
            }

            public void Dispose()
            {
                Owner.Context = null;
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
            using (Context = new ContextImpl(this))
            {
                Context.XTemplate = CreateCore();
                CreateXSourceTables();
                CreateXGridTables();
                return Context.XTemplate;
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
            DataTable fieldsData = Metadata.GetFieldsOnPageAsDataTable(page.Id);
            FieldDataTable fields = new FieldDataTable(fieldsData);
            IComparer<FieldDataRow> fieldComparer = new FieldDataRowComparer.EffectiveTabIndexAware(fields);
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
            foreach (string tableName in Context.SourceTableNames)
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
