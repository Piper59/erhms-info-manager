using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class DataPredicate
    {
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

    public static class DataPredicateExtensions
    {
        public static DataPredicate Combine(this IEnumerable<DataPredicate> @this)
        {
            return new DataPredicate(
                string.Join(" AND ", @this.Select(predicate => string.Format("({0})", predicate.Sql))),
                @this.SelectMany(predicate => predicate.Parameters));
        }
    }
}
