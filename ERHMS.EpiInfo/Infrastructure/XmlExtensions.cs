using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Infrastructure
{
    public static class XmlExtensions
    {
        public static XAttribute GetAttribute(this XElement @this, [CallerMemberName] string attributeName = null)
        {
            return @this.Attribute(attributeName);
        }

        public static T? GetAttributeValueOrNull<T>(this XElement @this, [CallerMemberName] string attributeName = null)
            where T : struct
        {
            if (@this.TryGetAttribute(attributeName, out XAttribute attribute))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(attribute.Value);
            }
            else
            {
                return null;
            }
        }

        public static void SetAttributeValue(this XElement @this, object value, [CallerMemberName] string attributeName = null)
        {
            if (value == null || value is DBNull)
            {
                value = "";
            }
            @this.SetAttributeValue(attributeName, value);
        }

        public static bool TryGetAttribute(this XElement @this, string attributeName, out XAttribute attribute)
        {
            attribute = @this.Attribute(attributeName);
            return attribute != null && attribute.Value != "";
        }
    }
}
