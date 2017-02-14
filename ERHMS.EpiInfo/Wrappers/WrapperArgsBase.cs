using ERHMS.Utility;
using System;
using System.ComponentModel;
using System.Reflection;

namespace ERHMS.EpiInfo.Wrappers
{
    public class WrapperArgsBase
    {
        private const BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public;

        public static object Parse(Type type, string value)
        {
            QueryString queryString = new QueryString(value);
            object args = Activator.CreateInstance(type);
            foreach (PropertyInfo property in type.GetProperties(PropertyFlags))
            {
                if (queryString.ContainsKey(property.Name))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                    property.SetValue(args, converter.ConvertFromString(queryString.Get(property.Name)), null);
                }
            }
            return args;
        }

        protected WrapperArgsBase() { }

        public override string ToString()
        {
            QueryString queryString = new QueryString();
            foreach (PropertyInfo property in GetType().GetProperties(PropertyFlags))
            {
                queryString.Set(property.Name, property.GetValue(this, null));
            }
            return queryString.ToString();
        }
    }
}
