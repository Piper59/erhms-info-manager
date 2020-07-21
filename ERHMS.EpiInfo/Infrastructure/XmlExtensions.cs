using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Infrastructure
{
    public static class XmlExtensions
    {
        public static XAttribute GetAttributeEx(this XElement @this, [CallerMemberName] string attributeName = null)
        {
            return @this.Attribute(attributeName);
        }

        public static void SetAttributeValueEx(this XElement @this, object value, [CallerMemberName] string attributeName = null)
        {
            if (value == null || value is DBNull)
            {
                value = "";
            }
            @this.SetAttributeValue(attributeName, value);
        }
    }
}
