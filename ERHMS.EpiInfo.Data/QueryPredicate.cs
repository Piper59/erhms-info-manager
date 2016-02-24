using Epi.Data;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class QueryPredicate
    {
        public static QueryPredicate Combine(IEnumerable<QueryPredicate> predicates)
        {
            IEnumerable<string> sqls = predicates.Select(predicate => string.Format("({0})", predicate.Sql));
            IEnumerable<QueryParameter> parameters = predicates.SelectMany(predicate => predicate.Parameters);
            QueryPredicate combined = new QueryPredicate(string.Join(" AND ", sqls));
            combined.Parameters.AddRange(parameters);
            return combined;
        }

        public string Sql { get; set; }
        public ICollection<QueryParameter> Parameters { get; set; }

        public QueryPredicate(string sql)
        {
            Sql = sql;
            Parameters = new List<QueryParameter>();
        }

        public void AddParameter(string name, DbType type, object value)
        {
            Parameters.Add(new QueryParameter(name, type, value));
        }
    }
}
