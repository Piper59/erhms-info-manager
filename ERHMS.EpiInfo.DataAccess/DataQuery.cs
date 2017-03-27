using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataQuery
    {
        public DataTransaction Transaction { get; private set; }
        public string Sql { get; private set; }
        public IEnumerable<DataParameter> Parameters { get; private set; }

        public DataQuery(string sql, IEnumerable<DataParameter> parameters, DataTransaction transaction = null)
        {
            Transaction = transaction;
            Sql = sql;
            Parameters = parameters.ToList();
        }
    }
}
