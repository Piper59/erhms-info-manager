using System.Collections.Generic;
using System.Data.Common;

namespace ERHMS.Utility
{
    public static class DataExtensions
    {
        public static string ToSafeString(this DbConnectionStringBuilder @this)
        {
            ICollection<string> properties = new List<string>();
            foreach (KeyValuePair<string, object> property in @this)
            {
                object value = property.Key.ToLower().Contains("password") ? "?" : property.Value;
                properties.Add(string.Format("{0} = {1}", property.Key, value));
            }
            return string.Join(", ", properties);
        }

        public static string ToSafeString(string connectionString)
        {
            return new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            }.ToSafeString();
        }
    }
}
