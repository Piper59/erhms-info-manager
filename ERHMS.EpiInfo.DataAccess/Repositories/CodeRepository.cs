using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.DataAccess
{
    public class CodeRepository : RepositoryBase
    {
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }

        public CodeRepository(IDataDriver driver, string tableName, string columnName)
            : base(driver)
        {
            TableName = tableName;
            ColumnName = columnName;
        }

        public IEnumerable<string> Select()
        {
            string sql = string.Format("SELECT {0} FROM {1}", Driver.Escape(ColumnName), Driver.Escape(TableName));
            foreach (DataRow row in Driver.ExecuteQuery(sql).Rows)
            {
                yield return row.Field<string>(ColumnName);
            }
        }
    }
}
