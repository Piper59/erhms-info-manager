using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class DynamicExtensions
    {
        public static string Serialize(object value)
        {
            IDictionary<string, object> properties;
            if (value == null)
            {
                properties = null;
            }
            else
            {
                properties = value as IDictionary<string, object>;
                if (properties == null)
                {
                    properties = new Dictionary<string, object>();
                    foreach (PropertyInfo property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (property.GetIndexParameters().Length > 0)
                        {
                            continue;
                        }
                        properties.Add(property.Name, property.GetValue(value, null));
                    }
                }
                else
                {
                    properties = new Dictionary<string, object>(properties);
                }
            }
            return Base64Extensions.ToBase64String(properties);
        }

        public static ExpandoObject Deserialize(string value)
        {
            IDictionary<string, object> properties = (IDictionary<string, object>)Base64Extensions.FromBase64String(value);
            if (properties == null)
            {
                return null;
            }
            else
            {
                IDictionary<string, object> result = new ExpandoObject();
                foreach (KeyValuePair<string, object> property in properties)
                {
                    result.Add(property.Key, property.Value);
                }
                return (ExpandoObject)result;
            }
        }
    }
}
