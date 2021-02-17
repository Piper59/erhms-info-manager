using Epi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XProject : XElement
    {
        public static XProject Create(Project project)
        {
            XProject xProject = new XProject
            {
                Id = project.Id,
                Name = project.Name,
                Location = project.Location,
                Description = project.Description,
                EpiVersion = null,
                CreateDate = project.CreateDate
            };
            return xProject;
        }

        public Guid? Id
        {
            get { return this.GetAttributeValueOrNull<Guid>(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string Location
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string EpiVersion
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public DateTime? CreateDate
        {
            get { return this.GetAttributeValueOrNull<DateTime>(); }
            set { this.SetOrClearAttributeValue(value?.ToString(XTemplate.DateFormat)); }
        }

        public IEnumerable<XView> XViews => Elements(ElementNames.View).Cast<XView>();
        public IEnumerable<XField> XFields => XViews.SelectMany(xView => xView.XFields);

        public XProject()
            : base(ElementNames.Project) { }

        public XProject(XElement element)
            : this()
        {
            Add(element.Attributes());
            Add(element.Elements(ElementNames.View).Select(child => new XView(child)));
        }
    }
}
