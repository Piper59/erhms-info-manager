using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataParameterCollection : IEnumerable<DataParameter>
    {
        private IDataDriver driver;
        private ICollection<DataParameter> parameters;

        public DataParameterCollection(IDataDriver driver)
        {
            this.driver = driver;
            parameters = new List<DataParameter>();
        }

        public DataParameter AddByValue(object value)
        {
            DataParameter parameter = new DataParameter(driver.GetParameterName(parameters.Count), value);
            parameters.Add(parameter);
            return parameter;
        }

        public string Format(string format)
        {
            return string.Format(format, parameters.Select(parameter => parameter.Name).ToArray());
        }

        public DataPredicate ToPredicate(string format)
        {
            return new DataPredicate(Format(format), this);
        }

        public IEnumerator<DataParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)parameters).GetEnumerator();
        }
    }
}
