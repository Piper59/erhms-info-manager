using System.Data.Common;
using System.Linq;
using Property = System.Collections.Generic.KeyValuePair<string, object>;

namespace ERHMS.Utility
{
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
