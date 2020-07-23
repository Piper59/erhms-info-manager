using Epi;
using ERHMS.EpiInfo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XProject : XElement
    {
        private static readonly ICollection<string> ConfigurationSettingNames = new string[]
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

        public static XProject Create(Project project)
        {
            XProject xProject = new XProject
            {
                Id = project.Id,
                Name = project.Name,
                Location = project.Location,
                Description = project.Description,
                EpiVersion = project.EpiVersion,
                CreateDate = project.CreateDate
            };
            xProject.OnCreated(TemplateLevel.Project);
            return xProject;
        }

        public Guid? Id
        {
            get
            {
                if (Guid.TryParse((string)this.GetAttributeEx(), out Guid value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value);
            }
        }

        public new string Name
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public string Location
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public string EpiVersion
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public DateTime? CreateDate
        {
            get
            {
                if (DateTime.TryParse((string)this.GetAttributeEx(), out DateTime value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                this.SetAttributeValueEx(value?.ToString(XTemplate.DateFormat));
            }
        }

        public IEnumerable<XView> XViews => Elements().OfType<XView>();
        public IEnumerable<XField> XFields => Descendants().OfType<XField>();

        public XProject()
            : base(ElementNames.Project) { }

        public XProject(XElement element, TemplateLevel level)
            : this()
        {
            Add(element.Attributes());
            foreach (XElement xView in element.Elements(ElementNames.View))
            {
                Add(new XView(xView));
            }
            OnCreated(level);
        }

        private void OnCreated(TemplateLevel level)
        {
            if (level == TemplateLevel.Project)
            {
                Id = null;
                Location = null;
                CreateDate = null;
            }
            foreach (string settingName in ConfigurationSettingNames)
            {
                Attribute(settingName)?.Remove();
            }
        }
    }
}
