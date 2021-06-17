using Dapper;
using System.Collections;
using System.Collections.Generic;

namespace ERHMS.Data.Querying
{
    public class ParameterCollection : DynamicParameters, IEnumerable<string>
    {
        public void Add(string name, object value)
        {
            base.Add(name, value);
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
