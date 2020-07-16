using Epi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public partial class XTemplate
    {
        public static XTemplate Construct(Project project)
        {
            XTemplate xTemplate = new XTemplate(TemplateLevel.Project, project.Metadata)
            {
                Name = project.Name,
                Description = project.Description
            };
            xTemplate.Add(XProject.Construct(project));
            xTemplate.AddSourceTables();
            xTemplate.AddGridTables();
            xTemplate.AddBackgroundsTable();
            return xTemplate;
        }

        public static XTemplate Construct(View view)
        {
            XTemplate xTemplate = new XTemplate(TemplateLevel.View, view.GetMetadata())
            {
                Name = view.Name
            };
            xTemplate.Add(XProject.Construct(view));
            xTemplate.RemoveRelateFields();
            xTemplate.AddSourceTables();
            xTemplate.AddGridTables();
            return xTemplate;
        }

        public static XTemplate Construct(Page page)
        {
            XTemplate xTemplate = new XTemplate(TemplateLevel.Page, page.GetMetadata())
            {
                Name = page.Name
            };
            xTemplate.Add(XProject.Construct(page));
            xTemplate.RemoveRelateFields();
            xTemplate.AddSourceTables();
            xTemplate.AddGridTables();
            return xTemplate;
        }

        private void AddSourceTables()
        {
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in XProject.XFields)
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
                DataTable table = Metadata.GetCodeTableData(tableName);
                table.TableName = tableName;
                Add(XTable.Construct(ElementNames.SourceTable, table));
            }
        }

        private void AddGridTables()
        {
            foreach (XField xField in XProject.XFields)
            {
                if (xField.FieldType != MetaFieldType.Grid)
                {
                    continue;
                }
                DataTable table = Metadata.GetGridColumns(xField.FieldId);
                table.TableName = xField.Name;
                Add(XTable.Construct(ElementNames.GridTable, table));
            }
        }

        private void AddBackgroundsTable()
        {
            DataTable table = new DataTable(ElementNames.BackgroundsTable);
            Add(XTable.Construct(ElementNames.BackgroundsTable, table));
        }
    }
}
