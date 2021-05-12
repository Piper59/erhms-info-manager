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
            return new XProject
            {
                Id = project.Id,
                Name = project.Name,
                Location = project.Location,
                Description = project.Description,
                EpiVersion = null,
                CreateDate = project.CreateDate
            };
        }

        public Guid? Id
        {
            get { return this.GetAttributeValue<Guid>(); }
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
            get { return this.GetAttributeValue<DateTime>(); }
            set { this.SetAttributeValue(value?.ToString(XTemplate.DateFormat)); }
        }

        public IEnumerable<XView> XViews => Elements(ElementNames.View).Cast<XView>();
        public IEnumerable<XField> XFields => XViews.SelectMany(xView => xView.XFields);

        public XProject()
            : base(ElementNames.Project) { }

        public XProject(XElement element)
            : this()
        {
            if (element.Name != ElementNames.Project)
            {
                throw new ArgumentException($"Unexpected element name '{element.Name}'.");
            }
            Add(element.Attributes());
            Add(element.Elements(ElementNames.View).Select(child => new XView(child)));
        }

        public void Canonize(TemplateLevel level)
        {
            if (level == TemplateLevel.Project)
            {
                ReplaceAttributes(
                    new XAttribute(nameof(Id), ""),
                    this.CopyAttribute(nameof(Name)),
                    new XAttribute(nameof(Location), ""),
                    this.CopyAttribute(nameof(Description)),
                    this.CopyAttribute(nameof(EpiVersion)),
                    new XAttribute(nameof(CreateDate), "")));
            }
            else
            {
                RemoveAttributes();
            }
        }
    }
}
