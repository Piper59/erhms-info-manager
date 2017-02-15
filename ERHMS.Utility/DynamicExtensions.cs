using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class DynamicExtensions
    {
        public static string Serialize(object value)
        {
            IDictionary<string, object> properties = new Dictionary<string, object>();
            foreach (PropertyInfo property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                properties.Add(property.Name, property.GetValue(value, null));
            }
            return ConvertExtensions.ToBase64String(properties);
        }

        public static ExpandoObject Deserialize(string value)
        {
            ExpandoObject expandoObj = new ExpandoObject();
            IDictionary<string, object> properties = (IDictionary<string, object>)ConvertExtensions.FromBase64String(value);
            foreach (KeyValuePair<string, object> property in properties)
            {
                ((IDictionary<string, object>)expandoObj).Add(property.Key, property.Value);
            }
            return expandoObj;
        }
    }
}
