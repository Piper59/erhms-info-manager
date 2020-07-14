using Epi;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
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

        private IMetadataProvider Metadata { get; }

        private XTemplate(TemplateLevel level, IMetadataProvider metadata)
            : base(ElementNames.Template)
        {
            Name = "";
            Description = "";
            CreateDate = ConfigurationExtensions.CompatibilityMode ? DateTime.Now : (DateTime?)null;
            Level = level;
            Metadata = metadata;
        }

        public XTemplate(Project project)
            : this(TemplateLevel.Project, project.Metadata)
        {
            Log.Default.Debug("Creating project template");
            Name = project.Name;
            Description = project.Description;
            Add(new XProject(project));
            AddCodeTables();
            AddGridTables();
            AddBackgroundsTable();
        }

        public XTemplate(View view)
            : this(TemplateLevel.View, view.GetMetadata())
        {
            Log.Default.Debug("Creating view template");
            Name = view.Name;
            Add(new XProject(view));
            RemoveRelateFields();
            AddCodeTables();
            AddGridTables();
        }

        public XTemplate(Page page)
            : this(TemplateLevel.Page, page.GetMetadata())
        {
            Log.Default.Debug("Creating page template");
            Name = page.Name;
            Add(new XProject(page));
            RemoveRelateFields();
            AddCodeTables();
            AddGridTables();
        }

        public XTemplate(XElement element)
            : base(ElementNames.Template)
        {
            Add(element.Attributes());
            if (!IsLevelSupported(Level))
            {
                throw new NotSupportedException();
            }
            XElement xProject = element.Elements(ElementNames.Project).Single();
            Add(new XProject(xProject));
            foreach (string elementName in ElementNames.Tables)
            {
                foreach (XElement xTable in element.Elements(elementName))
                {
                    Add(new XTable(xTable));
                }
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
