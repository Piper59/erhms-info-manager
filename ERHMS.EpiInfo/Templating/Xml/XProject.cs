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
        internal static readonly IReadOnlyCollection<string> ConfigurationSettingNames = new string[]
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
            Configuration configuration = Configuration.GetNewInstance();
            foreach (string settingName in ConfigurationSettingNames)
            {
                xProject.SetAttributeValue(configuration.Settings[settingName], settingName);
            }
            return xProject;
        }

        public Guid? Id
        {
            get { return this.GetAttributeValueOrNull<Guid>(); }
            set { this.SetAttributeValue(value); }
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
            get { return this.GetAttributeValueOrNull<DateTime>(); }
            set { this.SetAttributeValue(value?.ToString(XTemplate.DateFormat)); }
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
        }

        public void RemoveRelateFields()
        {
            foreach (XField xField in XFields.ToList())
            {
                if (xField.FieldType == MetaFieldType.Relate)
                {
                    xField.Remove();
                }
            }
            foreach (XView xView in XViews)
            {
                xView.IsRelatedView = false;
            }
        }
    }
}
