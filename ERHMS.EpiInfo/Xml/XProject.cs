using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XProject : XElement
    {
        private static readonly ICollection<string> ConfigurationAttributeNames = new string[]
        {
            "ControlFontBold",
            "ControlFontItalics",
            "ControlFontName",
            "ControlFontSize",
            "DefaultLabelAlign",
            "DefaultPageHeight",
            "DefaultPageOrientation",
            "DefaultPageWidth",
            "EditorFontBold",
            "EditorFontItalics",
            "EditorFontName",
            "EditorFontSize"
        };

        public Guid? Id
        {
            get
            {
                if (Guid.TryParse((string)this.GetAttribute(), out Guid result))
                {
                    return result;
                }
                return null;
            }
            private set
            {
                this.SetAttributeValue(value);
            }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string Location
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string EpiVersion
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
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
            private set
            {
                this.SetAttributeValue(value?.ToString(XmlExtensions.DateFormat));
            }
        }

        private XProject()
            : base(ElementNames.Project) { }

        public XProject(Project project)
            : this()
        {
            Log.Default.Debug($"Adding project: {project.Name}");
            Id = null;
            Name = project.Name;
            Location = "";
            Description = project.Description;
            EpiVersion = project.EpiVersion;
            CreateDate = null;
            if (ConfigurationExtensions.CompatibilityMode)
            {
                Configuration configuration = Configuration.GetNewInstance();
                foreach (string attributeName in ConfigurationAttributeNames)
                {
                    this.SetAttributeValue(configuration.Settings[attributeName], attributeName);
                }
            }
            Add(
                new XElement("CollectedData",
                    new XElement("Database",
                        new XAttribute("Source", ""),
                        new XAttribute("DataDriver", "")
                    )
                ),
                new XElement("Metadata",
                    new XAttribute("Source", "")
                ),
                new XElement("EnterMakeviewInterpreter",
                    new XAttribute("Source", project.EnterMakeviewIntepreter)
                )
            );
            foreach (View view in project.Views)
            {
                Add(new XView(view));
            }
        }

        public XProject(View view)
            : this()
        {
            Add(new XView(view));
        }

        public XProject(Page page)
            : this()
        {
            Add(new XView(page));
        }

        public XProject(XElement element)
            : base(ElementNames.Project)
        {
            Add(element.Attributes());
            foreach (XElement xView in element.Elements(ElementNames.View))
            {
                Add(new XView(xView));
            }
        }
    }
}
