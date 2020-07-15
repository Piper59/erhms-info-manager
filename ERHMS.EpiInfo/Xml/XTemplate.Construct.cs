using Epi;
using ERHMS.Utility;
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
            Log.Default.Debug("Constructing project template");
            XTemplate xTemplate = new XTemplate(TemplateLevel.Project, project.Metadata)
            {
                Name = project.Name,
                Description = project.Description
            };
            xTemplate.Add(XProject.Construct(project));
            xTemplate.AddCodeTables();
            xTemplate.AddGridTables();
            xTemplate.AddBackgroundsTable();
            return xTemplate;
        }

        public static XTemplate Construct(View view)
        {
            Log.Default.Debug("Constructing view template");
            XTemplate xTemplate = new XTemplate(TemplateLevel.Project, view.GetMetadata())
            {
                Name = view.Name
            };
            xTemplate.Add(XProject.Construct(view));
            xTemplate.RemoveRelateFields();
            xTemplate.AddCodeTables();
            xTemplate.AddGridTables();
            return xTemplate;
        }

        public static XTemplate Construct(Page page)
        {
            Log.Default.Debug("Constructing page template");
            XTemplate xTemplate = new XTemplate(TemplateLevel.Project, page.GetMetadata())
            {
                Name = page.Name
            };
            xTemplate.Add(XProject.Construct(page));
            xTemplate.RemoveRelateFields();
            xTemplate.AddCodeTables();
            xTemplate.AddGridTables();
            return xTemplate;
        }

        private void RemoveRelateFields()
        {
            ICollection<XField> xFields = XFields.Where(xField => xField.FieldType == MetaFieldType.Relate).ToList();
            foreach (XField xField in xFields)
            {
                xField.Remove();
            }
        }

        private void AddCodeTables()
        {
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in XFields)
            {
                if (xField.FieldType.HasCodeTable())
                {
                    AddCodeTable(xField.SourceTableName, tableNames);
                }
                else if (xField.FieldType == MetaFieldType.Grid)
                {
                    DataTable gridColumns = Metadata.GetGridColumns(xField.FieldId);
                    foreach (DataRow gridColumn in gridColumns.Rows)
                    {
                        if (!gridColumn.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID).HasCodeTable())
                        {
                            continue;
                        }
                        string tableName = gridColumn.Field<string>(ColumnNames.SOURCE_TABLE_NAME);
                        AddCodeTable(tableName, tableNames);
                    }
                }
            }
        }

        private void AddCodeTable(string tableName, ISet<string> tableNames)
        {
            if (string.IsNullOrEmpty(tableName) || tableNames.Contains(tableName))
            {
                return;
            }
            DataTable table = Metadata.GetCodeTableData(tableName);
            table.TableName = tableName;
            Add(XTable.Construct(ElementNames.CodeTable, table));
            tableNames.Add(tableName);
        }

        private void AddGridTables()
        {
            foreach (XField xField in XFields)
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
