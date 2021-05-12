using System;
using System.Text;

namespace ERHMS.Data
{
    public class QueryInfo
    {
        public string TableSource { get; set; }
        public string Clauses { get; set; }

        public string GetSql(string selectList)
        {
            if (selectList == null)
            {
                throw new ArgumentNullException(nameof(selectList));
            }
            StringBuilder sql = new StringBuilder();
            sql.Append($"SELECT {selectList}");
            if (TableSource != null)
            {
                sql.Append($" FROM {TableSource}");
            }
            if (Clauses != null)
            {
                sql.Append($" {Clauses}");
            }
            sql.Append(";");
            return sql.ToString();
        }

        public ParameterCollection Parameters { get; set; }
    }
}
