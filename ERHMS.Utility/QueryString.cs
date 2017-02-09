using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ERHMS.Utility
{
    public class QueryString : IEnumerable<KeyValuePair<string, string>>
    {
        public static QueryString Parse(string value)
        {
            return new QueryString(HttpUtility.ParseQueryString(value));
        }

        private NameValueCollection @base;

        public int Count
        {
            get { return @base.Count; }
        }

        public IEnumerable<string> Keys
        {
            get { return @base.Cast<string>(); }
        }

        public IEnumerable<string> Values
        {
            get { return Keys.Select(key => Get(key)); }
        }

        public IEnumerable<KeyValuePair<string, string>> Pairs
        {
            get { return Keys.Select(key => new KeyValuePair<string, string>(key, Get(key))); }
        }

        private QueryString(NameValueCollection @base)
        {
            this.@base = @base;
        }

        public QueryString()
            : this(HttpUtility.ParseQueryString("")) { }

        public void Clear()
        {
            @base.Clear();
        }

        public bool ContainsKey(string key)
        {
            return Keys.Contains(key);
        }

        public string Get(string key)
        {
            return @base.Get(key);
        }

        public void Remove(string key)
        {
            @base.Remove(key);
        }

        public void Set(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key cannot be null.");
            }
            if (value == null)
            {
                throw new ArgumentNullException("Value cannot be null.");
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

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
