using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ERHMS.Utility
{
    using Pair = KeyValuePair<string, string>;

    public class QueryString : IEnumerable<Pair>
    {
        private NameValueCollection @base;

        public QueryString(string value = "")
        {
            @base = HttpUtility.ParseQueryString(value);
        }

        public bool ContainsKey(string key)
        {
            return @base.Cast<string>().Contains(key);
        }

        public string Get(string key)
        {
            return @base.Get(key);
        }

        public void Set(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            @base.Set(key, value);
        }

        public void Set(string key, object value)
        {
            Set(key, value.ToString());
        }

        public override string ToString()
        {
            return @base.ToString();
        }

        public IEnumerator<Pair> GetEnumerator()
        {
            return @base.Cast<string>()
                .Select(key => new Pair(key, @base.Get(key)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
