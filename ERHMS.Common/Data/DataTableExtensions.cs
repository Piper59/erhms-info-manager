using System;
using System.Data;

namespace ERHMS.Common.Data
{
    public static class DataTableExtensions
    {
        public static bool DeepEquals(this DataTable table1, DataTable table2)
        {
            if (table1.Columns.Count != table2.Columns.Count)
            {
                return false;
            }
            if (table1.Rows.Count != table2.Rows.Count)
            {
                return false;
            }
            foreach (DataColumn column1 in table1.Columns)
            {
                DataColumn column2 = table2.Columns[column1.ColumnName];
                if (column2 == null || column1.DataType != column2.DataType)
                {
                    return false;
                }
            }
            for (int index = 0; index < table1.Rows.Count; index++)
            {
                DataRow row1 = table1.Rows[index];
                DataRow row2 = table2.Rows[index];
                foreach (DataColumn column1 in table1.Columns)
                {
                    DataColumn column2 = table2.Columns[column1.ColumnName];
                    if (!Equals(row1[column1], row2[column2]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void SetColumnDataType(this DataTable @this, string columnName, Type dataType)
        {
            DataColumn source = @this.Columns[columnName];
            if (source.DataType == dataType)
            {
                return;
            }
            if (!@this.Columns.CanRemove(source))
            {
                throw new ArgumentException($"Cannot remove column '{columnName}'.", nameof(columnName));
            }
            DataColumn target = @this.Columns.Add(null, dataType);
            try
            {
                foreach (DataRow row in @this.Rows)
                {
                    if (!row.IsNull(source))
                    {
                        row[target] = Convert.ChangeType(row[source], dataType);
                    }
                }
            }
            catch
            {
                @this.Columns.Remove(target);
                throw;
            }
            int ordinal = source.Ordinal;
            source.Table.Columns.Remove(source);
            target.ColumnName = columnName;
            target.SetOrdinal(ordinal);
        }
    }
}
