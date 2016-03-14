using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ERHMS.EpiInfo.DataAccess
{
    public class CodeRepository : RepositoryBase
    {
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public bool Sorted { get; private set; }

        public CodeRepository(IDataDriver driver, string tableName, string columnName, bool sorted)
            : base(driver)
        {
            TableName = tableName;
            ColumnName = columnName;
            Sorted = sorted;
        }

        public IEnumerable<string> Select()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT {0} FROM {1}", Driver.Escape(ColumnName), Driver.Escape(TableName));
            if (Sorted)
            {
                sql.AppendFormat(" ORDER BY {0}", Driver.Escape(ColumnName));
            }
            foreach (DataRow row in Driver.ExecuteQuery(sql.ToString()).Rows)
            {
                yield return row.Field<string>(ColumnName);
            }
        }
    }
}
