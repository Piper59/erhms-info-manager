using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataQueryBuilder
    {
        private static readonly Regex ParameterPattern = new Regex(@"\{@}");

        public IDataDriver Driver { get; private set; }
        public DataTransaction Transaction { get; private set; }
        public StringBuilder Sql { get; private set; }
        public IList<object> Values { get; private set; }

        public DataQueryBuilder(IDataDriver driver, DataTransaction transaction = null)
        {
            Driver = driver;
            Transaction = transaction;
            Sql = new StringBuilder();
            Values = new List<object>();
        }

        public DataQuery GetQuery()
        {
            IList<DataParameter> parameters = new List<DataParameter>();
            for (int valueIndex = 0; valueIndex < Values.Count; valueIndex++)
            {
                parameters.Add(new DataParameter(Driver.GetParameterName(valueIndex), Values[valueIndex]));
            }
            int matchIndex = 0;
            string sql = ParameterPattern.Replace(Sql.ToString(), match => parameters[matchIndex++].Name);
            return new DataQuery(sql, parameters, Transaction);
        }
    }
}
