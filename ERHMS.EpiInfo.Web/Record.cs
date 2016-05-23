using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Web
{
    public class Record : Dictionary<string, string>
    {
        private static readonly ICollection<string> TrueValues = new string[] { "1", "Yes" };
        private static readonly ICollection<string> FalseValues = new string[] { "0", "No" };

        public string GlobalRecordId { get; set; }
        public string Passcode { get; set; }

        public object GetValue(string key, Type type)
        {
            string value = this[key];
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch
            {
                if (type == typeof(bool))
                {
                    if (TrueValues.Contains(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else if (FalseValues.Contains(value, StringComparison.OrdinalIgnoreCase))
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
                    return null;
                }
            }
        }

        public string GetUrl()
        {
            Configuration configuration = Configuration.GetNewInstance();
            string address = configuration.Settings.WebServiceEndpointAddress;
            int index = address.LastIndexOf('/');
            return string.Format("{0}/Survey/{1}", address.Substring(0, index), GlobalRecordId);
        }
    }
}
