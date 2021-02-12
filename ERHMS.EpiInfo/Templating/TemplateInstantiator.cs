using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Data;
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
        protected class ContextImpl : IMappingContext, IDisposable
        {
            private readonly IDictionary<int, int> viewIdMap = new Dictionary<int, int>();
            private readonly IDictionary<int, int> fieldIdMap = new Dictionary<int, int>();
            private readonly IDictionary<string, string> tableNameMap =
                new Dictionary<string, string>(NameComparer.Default);
            private readonly IDictionary<int, IDictionary<string, string>> fieldNameMapsByViewId =
                new Dictionary<int, IDictionary<string, string>>();

            public TemplateInstantiator Owner { get; }
            public IReadOnlyCollection<IFieldMapper> FieldMappers { get; }
            public TableNameUniquifier TableNameUniquifier { get; }
            public ViewNameUniquifier ViewNameUniquifier { get; }
            public PageNameUniquifier PageNameUniquifier { get; private set; }
            public FieldNameUniquifier FieldNameUniquifier { get; private set; }

            private View view;
            public View View
            {
                get
                {
                    return view;
                }
                set
                {
                    if (value == view)
                    {
                        return;
                    }
                    PageNameUniquifier = new PageNameUniquifier(value);
                    FieldNameUniquifier = new FieldNameUniquifier(value);
                    if (!fieldNameMapsByViewId.ContainsKey(value.Id))
                    {
                        fieldNameMapsByViewId[value.Id] = new Dictionary<string, string>(NameComparer.Default);
                    }
                    view = value;
                }
            }

            private readonly ICollection<Field> fields = new List<Field>();
            public IEnumerable<Field> Fields => fields;

            public ContextImpl(TemplateInstantiator owner)
            {
                Owner = owner;
                FieldMappers = FieldMapper.GetInstances(this).ToList();
                TableNameUniquifier = new TableNameUniquifier(owner.Metadata.Project);
                ViewNameUniquifier = new ViewNameUniquifier(owner.Metadata.Project);
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
                fieldNameMapsByViewId[view.Id][xField.Name] = field.Name;
                fields.Add(field);
            }

            public void OnError(Exception exception, out bool handled)
            {
                Log.Default.Warn(exception);
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
                return tableNameMap.TryGetValue(value, out result);
            }

            public bool MapFieldName(string value, out string result)
            {
                return fieldNameMapsByViewId[view.Id].TryGetValue(value, out result);
            }

            public void Dispose()
            {
                Owner.Context = null;
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
                string message = $"Template level '{xTemplate.Level}' does not match instantiator level '{Level}'.";
                throw new ArgumentException(message, nameof(xTemplate));
            }
            XTemplate = xTemplate;
            Metadata = metadata;
        }

        protected abstract void InstantiateCore();

        public void Instantiate()
        {
            using (Context = new ContextImpl(this))
            {
                InstantiateSourceTables();
                InstantiateCore();
                MapFieldProperties();
                InstantiateGridTables();
            }
        }

        private void InstantiateSourceTables()
        {
            foreach (XTable xTable in XTemplate.XSourceTables)
            {
                InstantiateSourceTable(xTable);
            }
        }

        private DataTable InstantiateSourceTable(XTable xTable)
        {
            Progress?.Report($"Adding source table: {xTable.TableName}");
            DataTable table = xTable.Instantiate();
            bool exists = false;
            if (Context.TableNameUniquifier.Exists(table.TableName))
            {
                if (table.DataEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    exists = true;
                }
                else
                {
                    table.TableName = Context.TableNameUniquifier.Uniquify(table.TableName);
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
            }
            Context.OnSourceTableInstantiated(xTable, table);
            return table;
        }

        protected View InstantiateView(Project project, XView xView, bool recursive)
        {
            Progress?.Report($"Adding view: {xView.Name}");
            View view = xView.Instantiate(project);
            if (Context.ViewNameUniquifier.Exists(view.Name))
            {
                view.Name = Context.ViewNameUniquifier.Uniquify(view.Name);
                Progress?.Report($"Renamed view: {view.Name}");
            }
            Metadata.InsertView(view);
            project.Views.Add(view);
            Context.OnViewInstantiated(xView, view);
            if (recursive)
            {
                Context.View = view;
                foreach (XPage xPage in xView.XPages)
                {
                    InstantiatePage(view, xPage);
                }
            }
            return view;
        }

        protected Page InstantiatePage(View view, XPage xPage)
        {
            Progress?.Report($"Adding page: {xPage.Name}");
            Page page = xPage.Instantiate(view);
            if (Context.PageNameUniquifier.Exists(page.Name))
            {
                page.Name = Context.PageNameUniquifier.Uniquify(page.Name);
                Progress?.Report($"Renamed page: {page.Name}");
            }
            page.Position = view.Pages.Count;
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
            if (Context.FieldNameUniquifier.Exists(field.Name))
            {
                field.Name = Context.FieldNameUniquifier.Uniquify(field.Name);
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
            Context.View.Fields.Add(field);
            Context.View.MustRefreshFieldCollection = false;
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
            DataTable gridTableData = xTable.Instantiate();
            GridColumnDataTable gridTable = new GridColumnDataTable(gridTableData);
            foreach (GridColumnDataRow gridColumn in gridTable)
            {
                if (gridColumn.IsMetadata())
                {
                    continue;
                }
                if (Context.MapFieldId(gridColumn.FieldId, out int fieldId))
                {
                    gridColumn.FieldId = fieldId;
                }
                if (gridColumn.SourceTableName != null
                    && Context.MapTableName(gridColumn.SourceTableName, out string tableName))
                {
                    gridColumn.SourceTableName = tableName;
                }
                Metadata.AddGridColumn(gridColumn);
            }
            return gridTable;
        }
    }
}
