using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ERHMS.Utility
{
    public static class XmlExtensions
    {
        public static bool HasAllAttributes(this XmlElement @this, params string[] names)
        {
            return names.All(name => @this.HasAttribute(name));
        }

        public static XmlElement ReadNextElement(this XmlReader @this, out bool empty)
        {
            while (@this.Read())
            {
                if (@this.NodeType == XmlNodeType.Element)
                {
                    empty = @this.IsEmptyElement;
                    XmlElement element = new XmlDocument().CreateElement(@this.Name);
                    while (@this.MoveToNextAttribute())
                    {
                        element.SetAttribute(@this.Name, @this.Value);
                    }
                    return element;
                }
            }
            empty = false;
            return null;
        }

        public static XmlElement ReadNextElement(this XmlReader @this)
        {
            bool empty;
            return @this.ReadNextElement(out empty);
        }

        public static IEnumerable<XmlElement> SelectElements(this XmlNode @this, string xpath)
        {
            return @this.SelectNodes(xpath).OfType<XmlElement>();
        }

        public static IEnumerable<XmlElement> SelectElements(this XmlNode @this, string xpath, XmlNamespaceManager nsmgr)
        {
            return @this.SelectNodes(xpath, nsmgr).OfType<XmlElement>();
        }

        public static XmlElement SelectSingleElement(this XmlNode @this, string xpath)
        {
            return @this.SelectElements(xpath).FirstOrDefault();
        }

        public static XmlElement SelectSingleElement(this XmlNode @this, string xpath, XmlNamespaceManager nsmgr)
        {
            return @this.SelectElements(xpath, nsmgr).FirstOrDefault();
        }
    }
}
