using Epi.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class QueryPredicate
    {
        public static QueryPredicate Combine(IEnumerable<QueryPredicate> predicates)
        {
            ICollection<QueryPredicate> predicateCollection = predicates.ToList();
            if (!predicateCollection.Any())
            {
                return null;
            }
            IEnumerable<string> sqls = predicateCollection.Select(predicate => string.Format("({0})", predicate.Sql));
            IEnumerable<QueryParameter> parameters = predicateCollection.SelectMany(predicate => predicate.Parameters);
            QueryPredicate combined = new QueryPredicate(string.Join(" AND ", sqls));
            foreach (QueryParameter parameter in parameters)
            {
                combined.Parameters.Add(parameter);
            }
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
