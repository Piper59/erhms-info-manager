using Dapper;
using System.Collections;
using System.Collections.Generic;

namespace ERHMS.Data
{
    public class ParameterCollection : DynamicParameters, IEnumerable<string>
    {
        public void Add(string parameterName, object value)
        {
            base.Add(parameterName, value);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ParameterNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
