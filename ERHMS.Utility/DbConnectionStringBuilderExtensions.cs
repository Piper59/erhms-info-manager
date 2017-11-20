using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ERHMS.Utility
{
    public static class DbConnectionStringBuilderExtensions
    {
        private static bool IsCensorable(KeyValuePair<string, object> property)
        {
            return property.Key.ContainsIgnoreCase("Password") || property.Key.EqualsIgnoreCase("Pwd");
        }

        private static string ToCensoredString(KeyValuePair<string, object> property)
        {
            return string.Format("{0}={1}", property.Key, IsCensorable(property) ? "?" : property.Value);
        }

        public static string GetCensoredConnectionString(this DbConnectionStringBuilder @this)
        {
            return string.Join(";", @this.Cast<KeyValuePair<string, object>>().Select(property => ToCensoredString(property)));
        }
    }
}
