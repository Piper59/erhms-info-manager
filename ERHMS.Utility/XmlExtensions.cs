﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ERHMS.Utility
{
    public static class XmlExtensions
    {
        public static bool HasAllAttributes(this XmlElement @this, params string[] names)
        {
            foreach (string name in names)
            {
                if (!@this.HasAttribute(name))
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
                    XmlElement element = new XmlDocument().CreateElement(@this.Name);
                    while (@this.MoveToNextAttribute())
                    {
                        element.SetAttribute(@this.Name, @this.Value);
                    }
                    return element;
                }
            }
            return null;
        }

        public static IEnumerable<XmlElement> SelectElements(this XmlNode @this, string xpath)
        {
            return @this.SelectNodes(xpath).OfType<XmlElement>();
        }

        public static XmlElement SelectSingleElement(this XmlNode @this, string xpath)
        {
            return @this.SelectElements(xpath).FirstOrDefault();
        }
    }
}