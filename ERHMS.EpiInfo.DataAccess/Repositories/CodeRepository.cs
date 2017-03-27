using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.DataAccess
{
    public class CodeRepository
    {
        public IDataDriver Driver { get; private set; }
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public bool Sorted { get; private set; }

        public CodeRepository(IDataDriver driver, string tableName, string columnName, bool sorted)
        {
            Log.Logger.DebugFormat("Opening code repository: {0}", tableName);
            Driver = driver;
            TableName = tableName;
            ColumnName = columnName;
            Sorted = sorted;
        }

        public IEnumerable<string> Select()
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.AppendFormat("SELECT {0} FROM {1}", Driver.Escape(ColumnName), Driver.Escape(TableName));
            if (Sorted)
            {
                builder.Sql.AppendFormat(" ORDER BY {0}", Driver.Escape(ColumnName));
            }
            foreach (DataRow row in Driver.ExecuteQuery(builder.GetQuery()).Rows)
            {
                yield return row.Field<string>(ColumnName);
            }
        }
    }
}
