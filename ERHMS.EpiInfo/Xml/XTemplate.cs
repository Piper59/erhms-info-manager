using Epi;
using Epi.Data.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public partial class XTemplate : XElement
    {
        public static bool IsLevelSupported(TemplateLevel level)
        {
            switch (level)
            {
                case TemplateLevel.Project:
                case TemplateLevel.View:
                case TemplateLevel.Page:
                    return true;
                default:
                    return false;
            }
        }

        public static XTemplate Wrap(XElement element)
        {
            XTemplate xTemplate = new XTemplate();
            xTemplate.Add(element.Attributes());
            if (!IsLevelSupported(xTemplate.Level))
            {
                throw new NotSupportedException();
            }
            XElement xProject = element.Element(ElementNames.Project);
            xTemplate.Add(XProject.Wrap(xProject));
            if (xTemplate.Level != TemplateLevel.Project)
            {
                xTemplate.RemoveRelateFields();
            }
            foreach (string elementName in ElementNames.Tables)
            {
                foreach (XElement xTable in element.Elements(elementName))
                {
                    xTemplate.Add(XTable.Wrap(xTable));
                }
            }
            return xTemplate;
        }

        private IMetadataProvider Metadata { get; set; }

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
            get
            {
                if (DateTime.TryParse((string)this.GetAttribute(), out DateTime result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                this.SetAttributeValue(value?.ToString(XmlExtensions.DateFormat));
            }
        }

        public TemplateLevel Level
        {
            get { return TemplateLevelExtensions.Parse((string)this.GetAttribute()); }
            set { this.SetAttributeValue(value); }
        }

        public XProject XProject => Elements().OfType<XProject>().Single();
        public IEnumerable<XTable> XSourceTables => Elements(ElementNames.SourceTable).OfType<XTable>();
        public IEnumerable<XTable> XGridTables => Elements(ElementNames.GridTable).OfType<XTable>();

        private XTemplate()
            : base(ElementNames.Template) { }

        private XTemplate(TemplateLevel level, IMetadataProvider metadata)
            : this()
        {
            if (!IsLevelSupported(level))
            {
                throw new NotSupportedException();
            }
            Metadata = metadata;
            Name = "";
            Description = "";
            CreateDate = ConfigurationExtensions.CompatibilityMode ? DateTime.Now : (DateTime?)null;
            Level = level;
        }

        private void RemoveRelateFields()
        {
            ICollection<XField> xFields = XProject.XFields
                .Where(xField => xField.FieldType == MetaFieldType.Relate)
                .ToList();
            foreach (XField xField in xFields)
            {
                xField.Remove();
            }
            foreach (XView xView in XProject.XViews)
            {
                xView.IsRelatedView = false;
            }
        }

        public new void Save(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                Save(writer);
            }
        }
    }
}
