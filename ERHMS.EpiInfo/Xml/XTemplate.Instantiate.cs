using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public partial class XTemplate
    {
        private class InstantiationContext : IDisposable
        {
            public static InstantiationContext Current { get; private set; }

            public static IDisposable Create(Project project)
            {
                return new InstantiationContext(project);
            }

            public TableNameGenerator TableNameGenerator { get; }
            public ViewNameGenerator ViewNameGenerator { get; }
            public PageNameGenerator PageNameGenerator { get; private set; }
            public FieldNameGenerator FieldNameGenerator { get; private set; }
            public IDictionary<string, string> TableNameMap { get; }
            public IDictionary<int, int> ViewIdMap { get; }
            public IDictionary<int, int> FieldIdMap { get; }

            private InstantiationContext(Project project)
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

            public void Dispose()
            {
                Current = null;
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

        public void Instantiate(Project project)
        {
            if (Level == TemplateLevel.Page)
            {
                throw new ArgumentException();
            }
            InstantiateInternal(project);
        }

        public void Instantiate(View view)
        {
            if (Level != TemplateLevel.Page)
            {
                throw new ArgumentException();
            }
            InstantiateInternal(view.Project, view);
        }

        private void InstantiateInternal(Project project, View view = null)
        {
            Metadata = project.Metadata;
            using (InstantiationContext.Create(project))
            {
                foreach (XTable xTable in XSourceTables)
                {
                    InstantiateSourceTable(xTable);
                }
                ICollection<Field> fields = null;
                switch (Level)
                {
                    case TemplateLevel.Project:
                        foreach (XView xView in XProject.XViews)
                        {
                            InstantiateView(xView, project);
                        }
                        foreach (XView xView in XProject.XViews)
                        {
                            view = project.GetViewById(InstantiationContext.Current.ViewIdMap[xView.ViewId]);
                            InstantiationContext.Current.SetView(view);
                            fields = InstantiateFields(xView, view).ToList();
                        }
                        break;
                    case TemplateLevel.View:
                        {
                            XView xView = XProject.XViews.Single();
                            view = InstantiateView(xView, project);
                            InstantiationContext.Current.SetView(view);
                            fields = InstantiateFields(xView, view).ToList();
                        }
                        break;
                    case TemplateLevel.Page:
                        {
                            XView xView = XProject.XViews.Single();
                            XPage xPage = xView.XPages.Single();
                            InstantiationContext.Current.SetView(view);
                            Page page = InstantiatePage(xPage, view);
                            fields = InstantiateFields(xPage, page).ToList();
                            AddCheckCode(xView, view);
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
                foreach (Field field in fields)
                {
                    PostprocessField(field);
                }
                foreach (XTable xTable in XGridTables)
                {
                    InstantiateGridTable(xTable);
                }
            }
        }

        private void InstantiateSourceTable(XTable xTable)
        {
            DataTable table = xTable.Instantiate();
            if (InstantiationContext.Current.TableNameGenerator.Exists(table.TableName))
            {
                if (table.DataEquals(Metadata.GetCodeTableData(table.TableName)))
                {
                    return;
                }
                string original = table.TableName;
                string modified = InstantiationContext.Current.TableNameGenerator.MakeUnique(original);
                InstantiationContext.Current.TableNameMap[original] = modified;
                table.TableName = modified;
            }
            string[] columnNames = table.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();
            Metadata.CreateCodeTable(table.TableName, columnNames);
            Metadata.SaveCodeTableData(table, table.TableName, columnNames);
        }

        private View InstantiateView(XView xView, Project project)
        {
            View view = xView.Instantiate(project);
            if (InstantiationContext.Current.ViewNameGenerator.Exists(view.Name)
                || InstantiationContext.Current.ViewNameGenerator.Conflicts(view.Name))
            {
                view.Name = InstantiationContext.Current.ViewNameGenerator.MakeUnique(view.Name);
            }
            Metadata.InsertView(view);
            project.Views.Add(view);
            InstantiationContext.Current.ViewIdMap[xView.ViewId] = view.Id;
            return view;
        }

        private Page InstantiatePage(XPage xPage, View view)
        {
            Page page = xPage.Instantiate(view);
            if (InstantiationContext.Current.PageNameGenerator.Exists(page.Name))
            {
                page.Name = InstantiationContext.Current.PageNameGenerator.MakeUnique(page.Name);
            }
            page.Position = view.Pages.Count;
            Metadata.InsertPage(page);
            view.Pages.Add(page);
            return page;
        }

        private IEnumerable<Field> InstantiateFields(XView xView, View view)
        {
            return xView.XPages.SelectMany(xPage => InstantiateFields(xPage, InstantiatePage(xPage, view)));
        }

        private IEnumerable<Field> InstantiateFields(XPage xPage, Page page)
        {
            return xPage.XFields.Select(xField => InstantiateField(xField, page));
        }

        private Field InstantiateField(XField xField, Page page)
        {
            Field field = xField.Instantiate(page);
            if (InstantiationContext.Current.FieldNameGenerator.Exists(field.Name))
            {
                field.Name = InstantiationContext.Current.FieldNameGenerator.MakeUnique(field.Name);
            }
            MapSourceTableName(field as TableBasedDropDownField);
            MapRelatedViewId(field as RelatedViewField);
            field.SaveToDb();
            InstantiationContext.Current.FieldIdMap[xField.FieldId] = field.Id;
            return field;
        }

        private void MapSourceTableName(TableBasedDropDownField field)
        {
            if (field == null)
            {
                return;
            }
            if (InstantiationContext.Current.TableNameMap.TryGetValue(field.SourceTableName, out string tableName))
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
            field.RelatedViewID = InstantiationContext.Current.ViewIdMap[field.RelatedViewID];
        }

        private void AddCheckCode(XView xView, View view)
        {
            string checkCode = xView.CheckCode.Trim();
            if (view.CheckCode.Contains(checkCode))
            {
                return;
            }
            view.CheckCode = string.Concat(view.CheckCode.Trim(), "\n", checkCode).Trim();
            Metadata.UpdateView(view);
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
            field.SourceFieldId = InstantiationContext.Current.FieldIdMap[field.SourceFieldId];
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
            if (!InstantiationContext.Current.FieldIdMap.TryGetValue(fieldId, out fieldId))
            {
                return condition;
            }
            return string.Concat(columnName, RelateConditionSeparator, fieldId);
        }

        private void InstantiateGridTable(XTable xTable)
        {
            DataTable table = xTable.Instantiate();
            foreach (DataRow row in table.Rows)
            {
                if (IgnoredGridColumnNames.Contains(row.Field<string>(ColumnNames.NAME)))
                {
                    continue;
                }
                int fieldId = row.Field<int>(ColumnNames.FIELD_ID);
                row.SetField(ColumnNames.FIELD_ID, InstantiationContext.Current.FieldIdMap[fieldId]);
                Metadata.AddGridColumn(row);
            }
        }
    }
}
