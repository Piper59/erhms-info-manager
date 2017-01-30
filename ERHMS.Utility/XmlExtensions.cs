using System.Xml;

namespace ERHMS.Utility
{
    public static class XmlExtensions
    {
        public static bool HasAllAttributes(this XmlElement @this, params string[] attributeNames)
        {
            foreach (string attributeName in attributeNames)
            {
                if (!@this.HasAttribute(attributeName))
                {
                    return false;
                }
            }
            return true;
        }

        public static XmlElement ReadNextElement(this XmlReader @this)
        {
            while (@this.Read())
            {
                if (@this.NodeType == XmlNodeType.Element)
                {
                    XmlDocument document = new XmlDocument();
                    XmlElement element = document.CreateElement(@this.Name);
                    while (@this.MoveToNextAttribute())
                    {
                        element.SetAttribute(@this.Name, @this.Value);
                    }
                    return element;
                }
            }
            return null;
        }
    }
}
