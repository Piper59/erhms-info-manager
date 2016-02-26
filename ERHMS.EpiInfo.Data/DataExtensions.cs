using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    internal static class DataExtensions
    {
        public static void CopyDataTo(this DataRow @this, DataRow target)
        {
            foreach (DataColumn column in @this.Table.Columns)
            {
                if (target.Table.Columns.Contains(column.ColumnName))
                {
                    target[column.ColumnName] = @this[column];
                }
            }
        }

        public static void CopyRowsTo(this DataTable @this, DataTable target)
        {
            foreach (DataRow row in @this.Rows)
            {
                DataRow targetRow = target.NewRow();
                row.CopyDataTo(targetRow);
                target.Rows.Add(targetRow);
            }
        }

        public static void CopyDataTo(this DataTable @this, DataTable target)
        {
            foreach (DataRow row in @this.Rows)
            {
                object[] keys = @this.PrimaryKey
                    .Select(column => row[column])
                    .ToArray();
                DataRow targetRow = target.Rows.Find(keys);
                if (targetRow != null)
                {
                    row.CopyDataTo(targetRow);
                }
            }
        }
    }
}
