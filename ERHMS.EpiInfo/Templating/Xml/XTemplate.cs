using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTemplate : XElement
    {
        public const string DateFormat = "F";

        public static XmlWriterSettings GetXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
        }

        public static XTemplate Create(TemplateLevel level)
        {
            return new XTemplate
            {
                Name = null,
                Description = null,
                CreateDate = DateTime.Now,
                Level = level
            };
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public DateTime? CreateDate
        {
            get { return this.GetAttributeValueOrNull<DateTime>(); }
            set { this.SetAttributeValue(value?.ToString(DateFormat)); }
        }

        public TemplateLevel Level
        {
            get { return TemplateLevelExtensions.Parse((string)this.GetAttribute()); }
            set { this.SetAttributeValue(value); }
        }

        public XProject XProject => Elements().OfType<XProject>().Single();
        public IEnumerable<XTable> XSourceTables => Elements(ElementNames.SourceTable).OfType<XTable>();
        public IEnumerable<XTable> XGridTables => Elements(ElementNames.GridTable).OfType<XTable>();

        public XTemplate()
            : base(ElementNames.Template) { }

        public XTemplate(XElement element)
            : this()
        {
            Add(element.Attributes());
            XElement projectElement = element.Element(ElementNames.Project);
            Add(new XProject(projectElement));
            foreach (string elementName in ElementNames.Tables)
            {
                foreach (XElement tableElement in element.Elements(elementName))
                {
                    Add(new XTable(tableElement));
                }
            }
        }
    }
}
