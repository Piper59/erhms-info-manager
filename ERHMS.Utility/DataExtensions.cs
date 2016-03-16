using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ERHMS.Utility
{
    public static class DataExtensions
    {
        public static string ToSafeString(this DbConnectionStringBuilder @this)
        {
            return string.Join(", ", @this
                .Cast<KeyValuePair<string, object>>()
                .Where(property => !property.Key.ToLower().Contains("password"))
                .Select(property => string.Format("{0} = {1}", property.Key, property.Value)));
        }
    }
}
