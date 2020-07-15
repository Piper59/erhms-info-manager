using Epi.Data.Services;
using log4net;
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
        private static ILog Log { get; } = LogManager.GetLogger(nameof(ERHMS));

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
            Log.Debug("Wrapping template");
            XTemplate xTemplate = new XTemplate();
            xTemplate.Add(element.Attributes());
            if (!IsLevelSupported(xTemplate.Level))
            {
                throw new NotSupportedException();
            }
            XElement xProject = element.Element(ElementNames.Project);
            xTemplate.Add(XProject.Wrap(xProject));
            foreach (string elementName in ElementNames.Tables)
            {
                foreach (XElement xTable in element.Elements(elementName))
                {
                    xTemplate.Add(XTable.Wrap(xTable));
                }
            }
            return xTemplate;
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
            private set { this.SetAttributeValue(value); }
        }

        public XProject XProject => Elements().OfType<XProject>().Single();
        public IEnumerable<XField> XFields => Descendants().OfType<XField>();
        private IMetadataProvider Metadata { get; set; }

        private XTemplate()
            : base(ElementNames.Template) { }

        private XTemplate(TemplateLevel level, IMetadataProvider metadata)
            : this()
        {
            Name = "";
            Description = "";
            CreateDate = ConfigurationExtensions.CompatibilityMode ? DateTime.Now : (DateTime?)null;
            Level = level;
            Metadata = metadata;
        }

        public new void Save(Stream stream)
        {
            Log.Debug("Saving template");
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
