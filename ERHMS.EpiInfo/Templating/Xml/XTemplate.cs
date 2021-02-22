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

        public static XmlWriterSettings GetXmlWriterSettings(bool canonical)
        {
            return new XmlWriterSettings
            {
                NewLineOnAttributes = canonical,
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

        public static new XTemplate Load(string path)
        {
            return new XTemplate(XElement.Load(path));
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public DateTime? CreateDate
        {
            get { return this.GetAttributeValueOrNull<DateTime>(); }
            set { this.SetOrClearAttributeValue(value?.ToString(DateFormat)); }
        }

        public TemplateLevel Level
        {
            get { return TemplateLevelExtensions.Parse((string)this.GetAttribute()); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public XProject XProject => (XProject)Element(ElementNames.Project);
        public IEnumerable<XTable> XSourceTables => Elements(ElementNames.SourceTable).Cast<XTable>();
        public IEnumerable<XTable> XGridTables => Elements(ElementNames.GridTable).Cast<XTable>();

        public XTemplate()
            : base(ElementNames.Template) { }

        public XTemplate(XElement element)
            : this()
        {
            Add(element.Attributes());
            Add(new XProject(element.Element(ElementNames.Project)));
            Add(element.Elements(ElementNames.SourceTable).Select(child => new XTable(child)));
            Add(element.Elements(ElementNames.GridTable).Select(child => new XTable(child)));
            Add(element.Element(ElementNames.FieldFootprint));
        }

        public void Save(string path, bool canonical)
        {
            using (XmlWriter writer = XmlWriter.Create(path, GetXmlWriterSettings(canonical)))
            {
                Save(writer);
            }
        }

        public new void Save(string path)
        {
            Save(path, false);
        }
    }
}
