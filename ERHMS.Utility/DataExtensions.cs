using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ERHMS.Utility
{
    public static class DataExtensions
    {
        private static bool IsCensored(KeyValuePair<string, object> property)
        {
            return property.Key.ContainsIgnoreCase("Password") || property.Key.EqualsIgnoreCase("Pwd");
        }

        private static string FormatProperty(KeyValuePair<string, object> property)
        {
            return string.Format("{0}={1}", property.Key, IsCensored(property) ? "?" : property.Value);
        }

        public static string GetCensoredConnectionString(this DbConnectionStringBuilder @this)
        {
            return string.Join(";", @this.Cast<KeyValuePair<string, object>>().Select(property => FormatProperty(property)));
        }
    }
}
