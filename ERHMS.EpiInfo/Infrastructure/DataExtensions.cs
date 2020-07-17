using System;
using System.ComponentModel;
using System.Data;

namespace ERHMS.EpiInfo.Infrastructure
{
    internal static class DataExtensions
    {
        public static bool DataEquals(this DataTable table1, DataTable table2)
        {
            if (table1.Columns.Count != table2.Columns.Count)
            {
                return false;
            }
            if (table1.Rows.Count != table2.Rows.Count)
            {
                return false;
            }
            foreach (DataColumn column in table1.Columns)
            {
                if (!table2.Columns.Contains(column.ColumnName))
                {
                    return false;
                }
            }
            for (int index = 0; index < table1.Rows.Count; index++)
            {
                DataRow row1 = table1.Rows[index];
                DataRow row2 = table2.Rows[index];
                foreach (DataColumn column in table1.Columns)
                {
                    if (!Equals(row1[column.ColumnName], row2[column.ColumnName]))
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
            int ordinal = source.Ordinal;
            DataColumn target = @this.Columns.Add(null, dataType);
            target.SetOrdinal(ordinal);
            TypeConverter converter = TypeDescriptor.GetConverter(dataType);
            foreach (DataRow row in @this.Rows)
            {
                if (row.IsNull(source))
                {
                    continue;
                }
                row[target] = converter.ConvertFrom(row[source]);
            }
            source.Table.Columns.Remove(source);
            target.ColumnName = columnName;
        }
    }
}
