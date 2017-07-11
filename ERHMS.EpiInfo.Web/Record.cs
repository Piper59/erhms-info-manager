using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ERHMS.EpiInfo.Web
{
    public class Record : Dictionary<string, string>
    {
        private static readonly ICollection<string> TrueValues = new string[]
        {
            bool.TrueString,
            "1",
            "Yes"
        };
        private static readonly ICollection<string> FalseValues = new string[]
        {
            bool.FalseString,
            "0",
            "No"
        };

        public string GlobalRecordId { get; set; }
        public string Passcode { get; set; }

        public Record() { }

        public Record(object record)
        {
            foreach (PropertyInfo property in record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                this[property.Name] = Convert.ToString(property.GetValue(record, null));
            }
        }

        public object GetValue(string key, Type type)
        {
            string value = this[key];
            if (type == typeof(bool))
            {
                if (TrueValues.ContainsIgnoreCase(value))
                {
                    return true;
                }
                else if (FalseValues.ContainsIgnoreCase(value))
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch
                {
                    return null;
                }
            }
        }

        public Uri GetUrl()
        {
            Uri endpoint = new Uri(Configuration.GetNewInstance().Settings.WebServiceEndpointAddress);
            return new Uri(endpoint, string.Format("Survey/{0}", GlobalRecordId));
        }
    }
}
