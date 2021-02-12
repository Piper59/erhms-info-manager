using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public static class XElementExtensions
    {
        public static bool TryGetAttribute(this XElement @this, string attributeName, out XAttribute attribute)
        {
            attribute = @this.Attribute(attributeName);
            return attribute != null && attribute.Value != "";
        }

        public static XAttribute GetAttribute(this XElement @this, [CallerMemberName] string attributeName = null)
        {
            return @this.Attribute(attributeName);
        }

        public static TValue? GetAttributeValueOrNull<TValue>(
            this XElement @this,
            [CallerMemberName] string attributeName = null)
            where TValue : struct
        {
            if (@this.TryGetAttribute(attributeName, out XAttribute attribute))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(TValue));
                return (TValue)converter.ConvertFromString(attribute.Value);
            }
            else
            {
                return null;
            }
        }

        public static void SetOrClearAttributeValue(
            this XElement @this,
            object value,
            [CallerMemberName] string attributeName = null)
        {
            if (value == null || value is DBNull)
            {
                value = "";
            }
            @this.SetAttributeValue(attributeName, value);
        }
    }
}
