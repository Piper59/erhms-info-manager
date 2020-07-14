using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    internal static class XmlExtensions
    {
        public const string DateFormat = "F";

        public static XAttribute GetAttribute(this XElement @this, [CallerMemberName] string attributeName = null)
        {
            return @this.Attribute(attributeName);
        }

        public static void SetAttributeValue(this XElement @this, object value, [CallerMemberName] string attributeName = null)
        {
            if (value == null || value is DBNull)
            {
                value = "";
            }
            @this.SetAttributeValue(attributeName, value);
        }
    }
}
