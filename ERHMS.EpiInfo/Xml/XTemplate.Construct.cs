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
        private void RemoveRelateFields()
        {
            Log.Default.Debug("Removing relate fields");
            IEnumerable<XField> xFields = Descendants().OfType<XField>()
                .Where(xField => xField.FieldType == MetaFieldType.Relate)
                .ToList();
            foreach (XField xField in xFields)
            {
                xField.Remove();
            }
        }

        private void AddCodeTables()
        {
            Log.Default.Debug("Adding code tables");
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in Descendants().OfType<XField>())
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
            Add(new XTable(ElementNames.CodeTable, table));
            tableNames.Add(tableName);
        }

        private void AddGridTables()
        {
            Log.Default.Debug("Adding grid tables");
            foreach (XField xField in Descendants().OfType<XField>())
            {
                if (xField.FieldType != MetaFieldType.Grid)
                {
                    continue;
                }
                DataTable table = Metadata.GetGridColumns(xField.FieldId);
                table.TableName = xField.Name;
                Add(new XTable(ElementNames.GridTable, table));
            }
        }

        private void AddBackgroundsTable()
        {
            Log.Default.Debug("Adding backgrounds table");
            DataTable table = new DataTable(ElementNames.BackgroundsTable);
            Add(new XTable(ElementNames.BackgroundsTable, table));
        }
    }
}
