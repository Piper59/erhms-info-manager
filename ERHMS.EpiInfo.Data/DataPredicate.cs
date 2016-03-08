using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class DataPredicate
    {
        public static DataPredicate Combine(IEnumerable<DataPredicate> predicates)
        {
            return new DataPredicate(
                string.Join(" AND ", predicates.Select(predicate => string.Format("({0})", predicate.Sql))),
                predicates.SelectMany(predicate => predicate.Parameters));
        }

        public string Sql { get; set; }
        public ICollection<DataParameter> Parameters { get; set; }

        public DataPredicate(string sql, IEnumerable<DataParameter> parameters)
        {
            Sql = sql;
            Parameters = parameters.ToList();
        }

        public DataPredicate(string sql, params DataParameter[] parameters)
            : this(sql, (IEnumerable<DataParameter>)parameters)
        { }
    }
}
