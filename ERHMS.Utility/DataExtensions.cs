using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ERHMS.Utility
{
    using Property = KeyValuePair<string, object>;

    public static class DataExtensions
    {
        private static bool IsCensored(Property property)
        {
            return property.Key.ContainsIgnoreCase("Password") || property.Key.EqualsIgnoreCase("Pwd");
        }

        private static string FormatProperty(Property property)
        {
            return string.Format("{0}={1}", property.Key, IsCensored(property) ? "?" : property.Value);
        }

        public static string GetCensoredConnectionString(this DbConnectionStringBuilder @this)
        {
            return string.Join(";", @this.Cast<Property>().Select(property => FormatProperty(property)));
        }
    }
}
