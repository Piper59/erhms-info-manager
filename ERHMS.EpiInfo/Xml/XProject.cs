using Epi;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static XProject Construct(Project project)
        {
            XProject xProject = new XProject
            {
                Id = null,
                Name = project.Name,
                Location = "",
                Description = project.Description,
                EpiVersion = project.EpiVersion,
                CreateDate = null
            };
            if (ConfigurationExtensions.CompatibilityMode)
            {
                Configuration configuration = Configuration.GetNewInstance();
                foreach (string attributeName in ConfigurationAttributeNames)
                {
                    xProject.SetAttributeValue(configuration.Settings[attributeName], attributeName);
                }
            }
            xProject.Add(
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
                xProject.Add(XView.Construct(view));
            }
            return xProject;
        }

        public static XProject Construct(View view)
        {
            XProject xProject = new XProject();
            xProject.Add(XView.Construct(view));
            return xProject;
        }

        public static XProject Construct(Page page)
        {
            XProject xProject = new XProject();
            xProject.Add(XView.Construct(page));
            return xProject;
        }

        public static XProject Wrap(XElement element)
        {
            XProject xProject = new XProject();
            xProject.Add(element.Attributes());
            foreach (XElement xView in element.Elements(ElementNames.View))
            {
                xProject.Add(XView.Wrap(xView));
            }
            return xProject;
        }

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
            set
            {
                this.SetAttributeValue(value);
            }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string Location
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string EpiVersion
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

        public IEnumerable<XView> XViews => Elements().OfType<XView>();
        public IEnumerable<XField> XFields => Descendants().OfType<XField>();

        private XProject()
            : base(ElementNames.Project) { }
    }
}
