using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XTemplate : XElement
    {
        public static string DateFormat => "F";

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

        public static new XTemplate Load(Stream stream)
        {
            return new XTemplate(XElement.Load(stream));
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
            get { return this.GetAttributeValue<DateTime>(); }
            set { this.SetAttributeValue(value?.ToString(DateFormat)); }
        }

        public TemplateLevel Level
        {
            get { return TemplateLevelExtensions.Parse((string)this.GetAttribute()); }
            set { this.SetAttributeValue(value); }
        }

        public XProject XProject => (XProject)Element(ElementNames.Project);
        public IEnumerable<XTable> XSourceTables => Elements(ElementNames.SourceTable).Cast<XTable>();
        public IEnumerable<XTable> XGridTables => Elements(ElementNames.GridTable).Cast<XTable>();
        public bool Canonized { get; set; }

        public XTemplate()
            : base(ElementNames.Template) { }

        public XTemplate(XElement element)
            : this()
        {
            if (element.Name != ElementNames.Template)
            {
                throw new ArgumentException($"Unexpected element name '{element.Name}'.");
            }
            Add(element.Attributes());
            Add(new XProject(element.Element(ElementNames.Project)));
            Add(element.Elements(ElementNames.SourceTable).Select(child => new XTable(child)));
            Add(element.Elements(ElementNames.GridTable).Select(child => new XTable(child)));
            Add(element.Element(ElementNames.FieldFootprint));
        }

        public override void WriteTo(XmlWriter writer)
        {
            writer.Settings.OmitXmlDeclaration = true;
            writer.Settings.Indent = true;
            writer.Settings.NewLineOnAttributes = Canonized;
            base.WriteTo(writer);
        }

        public void Canonize()
        {
            Canonized = true;
            ReplaceAttributes(
                this.CopyAttribute(nameof(Name)),
                this.CopyAttribute(nameof(Description)),
                new XAttribute(nameof(CreateDate), ""),
                this.CopyAttribute(nameof(Level)));
        }
    }
}
