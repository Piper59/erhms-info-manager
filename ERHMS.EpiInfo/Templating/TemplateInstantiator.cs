using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common.Data;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Metadata;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating.Mapping;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public abstract class TemplateInstantiator
    {
        protected class ContextImpl : IMappingContext
        {
            private readonly IDictionary<int, int> viewIdMap = new Dictionary<int, int>();
            private readonly IDictionary<int, int> fieldIdMap = new Dictionary<int, int>();
            private readonly IDictionary<string, string> tableNameMap =
                new Dictionary<string, string>(NameComparer.Default);
            private readonly IDictionary<int, IDictionary<string, string>> fieldNameMapsByViewId =
                new Dictionary<int, IDictionary<string, string>>();

            public IMetadataProvider Metadata { get; }
            public IEnumerable<IFieldMapper> FieldMappers { get; }
            public TableNameUniquifier TableNames { get; }
            public ViewNameUniquifier ViewNames { get; }
            public PageNameUniquifier PageNames { get; private set; }
            public FieldNameUniquifier FieldNames { get; private set; }

            private View view;
            public View View
            {
                get
                {
                    return view;
                }
                set
                {
                    if (view != value)
                    {
                        PageNames = new PageNameUniquifier(value);
                        FieldNames = new FieldNameUniquifier(value);
                        if (!fieldNameMapsByViewId.ContainsKey(value.Id))
                        {
                            fieldNameMapsByViewId[value.Id] = new Dictionary<string, string>(NameComparer.Default);
                        }
                        view = value;
                    }
                }
            }

            private IDictionary<string, string> FieldNameMap => fieldNameMapsByViewId[View.Id];

            private readonly ICollection<Field> fields = new List<Field>();
            public IEnumerable<Field> Fields => fields;

            public ContextImpl(IMetadataProvider metadata)
            {
                Metadata = metadata;
                FieldMappers = FieldMapper.GetInstances(this).ToList();
                TableNames = new TableNameUniquifier(metadata.Project);
                ViewNames = new ViewNameUniquifier(metadata.Project);
            }

            public void OnSourceTableInstantiated(XTable xTable, DataTable table)
            {
                tableNameMap[xTable.TableName] = table.TableName;
            }

            public void OnViewInstantiated(XView xView, View view)
            {
                viewIdMap[xView.ViewId] = view.Id;
            }

            public void OnFieldInstantiated(XField xField, Field field)
            {
                fieldIdMap[xField.FieldId] = field.Id;
                FieldNameMap[xField.Name] = field.Name;
                fields.Add(field);
            }

            public void OnError(Exception exception, out bool handled)
            {
                Log.Instance.Warn(exception);
                handled = true;
            }

            public bool MapViewId(int value, out int result)
            {
                return viewIdMap.TryGetValue(value, out result);
            }

            public bool MapFieldId(int value, out int result)
            {
                return fieldIdMap.TryGetValue(value, out result);
            }

            public bool MapTableName(string value, out string result)
            {
                if (value == null)
                {
                    result = default(string);
                    return false;
                }
                return tableNameMap.TryGetValue(value, out result);
            }

            public bool MapFieldName(string value, out string result)
            {
                if (value == null)
                {
                    result = default(string);
                    return false;
                }
                return FieldNameMap.TryGetValue(value, out result);
            }
        }

        public abstract TemplateLevel Level { get; }
        public XTemplate XTemplate { get; }
        protected IMetadataProvider Metadata { get; }
        public IProgress<string> Progress { get; set; }
        protected ContextImpl Context { get; private set; }

        protected TemplateInstantiator(XTemplate xTemplate, IMetadataProvider metadata)
        {
            if (xTemplate.Level != Level)
            {
                throw new ArgumentException(
                    $"Unexpected template level '{xTemplate.Level}' (expected '{Level}').",
                    nameof(xTemplate));
            }
            XTemplate = xTemplate;
            Metadata = metadata;
        }

        protected abstract void InstantiateCore();

        public void Instantiate()
        {
            Context = new ContextImpl(Metadata);
            try
            {
                InstantiateSourceTables();
                InstantiateCore();
                MapFieldProperties();
                InstantiateGridTables();
            }
            finally
            {
                Context = null;
            }
        }

        protected View InstantiateViewCore(Project project, XView xView)
        {
            Progress?.Report($"Adding view: {xView.Name}");
            View view = xView.Instantiate(project);
            if (Context.ViewNames.Exists(view.Name))
            {
                view.Name = Context.ViewNames.Uniquify(view.Name);
                Progress?.Report($"Renamed view: {view.Name}");
            }
            Metadata.InsertView(view);
            project.Views.Add(view);
            Context.OnViewInstantiated(xView, view);
            return view;
        }

        protected View InstantiateView(Project project, XView xView)
        {
            View view = InstantiateViewCore(project, xView);
            Context.View = view;
            foreach (XPage xPage in xView.XPages)
            {
                InstantiatePage(view, xPage);
            }
            return view;
        }

        protected Page InstantiatePage(View view, XPage xPage)
        {
            Progress?.Report($"Adding page: {xPage.Name}");
            Page page = xPage.Instantiate(view);
            if (Context.PageNames.Exists(page.Name))
            {
                page.Name = Context.PageNames.Uniquify(page.Name);
                Progress?.Report($"Renamed page: {page.Name}");
            }
            Metadata.InsertPage(page);
            view.Pages.Add(page);
            foreach (XField xField in xPage.XFields)
            {
                InstantiateField(page, xField);
            }
            return page;
        }

        private Field InstantiateField(Page page, XField xField)
        {
            Progress?.Report($"Adding field: {xField.Name}");
            Field field = xField.Instantiate(page);
            if (Context.FieldNames.Exists(field.Name))
            {
                field.Name = Context.FieldNames.Uniquify(field.Name);
                Progress?.Report($"Renamed field: {field.Name}");
            }
            foreach (IFieldMapper mapper in Context.FieldMappers)
            {
                if (mapper.IsCompatible(field))
                {
                    mapper.SetProperties(xField, field);
                }
            }
            field.SaveToDb();
            Context.View.MustRefreshFieldCollection = false;
            Context.View.Fields.Add(field);
            Context.OnFieldInstantiated(xField, field);
            return field;
        }

        private void MapFieldProperties()
        {
            foreach (Field field in Context.Fields)
            {
                Context.View = field.View;
                bool changed = false;
                foreach (IFieldMapper mapper in Context.FieldMappers)
                {
                    if (mapper.IsCompatible(field) && mapper.MapProperties(field))
                    {
                        changed = true;
                    }
                }
                if (changed)
                {
                    field.SaveToDb();
                }
            }
        }

        private void InstantiateSourceTables()
        {
            foreach (XTable xTable in XTemplate.XSourceTables)
            {
                DataTable table = InstantiateSourceTable(xTable);
                Context.OnSourceTableInstantiated(xTable, table);
            }
        }

        private DataTable InstantiateSourceTable(XTable xTable)
        {
            Progress?.Report($"Adding source table: {xTable.TableName}");
            DataTable table = InstantiateTable(xTable);
            if (Context.TableNames.Exists(table.TableName))
            {
                if (table.DeepEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    return table;
                }
                else
                {
                    table.TableName = Context.TableNames.Uniquify(table.TableName);
                    Progress?.Report($"Renamed source table: {table.TableName}");
                }
            }
            string[] columnNames = table.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();
            Metadata.CreateCodeTable(table.TableName, columnNames);
            Metadata.SaveCodeTableData(table, table.TableName, columnNames);
            return table;
        }

        private void InstantiateGridTables()
        {
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                InstantiateGridTable(xTable);
            }
        }

        private GridColumnDataTable InstantiateGridTable(XTable xTable)
        {
            Progress?.Report($"Adding grid table: {xTable.TableName}");
            DataTable table = InstantiateTable(xTable);
            GridColumnDataTable gridColumns = new GridColumnDataTable(table);
            foreach (GridColumnDataRow gridColumn in gridColumns)
            {
                if (gridColumn.IsMetadata())
                {
                    continue;
                }
                if (Context.MapFieldId(gridColumn.FieldId, out int fieldId))
                {
                    gridColumn.FieldId = fieldId;
                }
                if (Context.MapTableName(gridColumn.SourceTableName, out string tableName))
                {
                    gridColumn.SourceTableName = tableName;
                }
                Metadata.AddGridColumn(gridColumn);
            }
            return gridColumns;
        }

        private DataTable InstantiateTable(XTable xTable)
        {
            DataTable table = xTable.Instantiate();
            foreach (XItem xItem in xTable.XItems)
            {
                table.Rows.Add(xItem.Instantiate(table));
            }
            return table;
        }
    }
}
