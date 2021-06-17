using Epi;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XPage : XElement
    {
        public static XPage Create(Page page)
        {
            return new XPage
            {
                PageId = page.Id,
                Name = page.Name,
                Position = page.Position,
                BackgroundId = page.BackgroundId,
                ViewId = page.GetView().Id
            };
        }

        public int PageId
        {
            get
            {
                return (int)this.GetAttribute();
            }
            set
            {
                this.SetAttributeValue(value);
                foreach (XField xField in XFields)
                {
                    xField.PageId = value;
                }
            }
        }

        public new string Name
        {
            get
            {
                return (string)this.GetAttribute();
            }
            set
            {
                this.SetAttributeValue(value);
                foreach (XField xField in XFields)
                {
                    xField.PageName = value;
                }
            }
        }

        public int Position
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int BackgroundId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int ViewId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public XView XView => (XView)Parent;
        public IEnumerable<XField> XFields => Elements(ElementNames.Field).Cast<XField>();

        public XPage()
            : base(ElementNames.Page) { }

        public XPage(XElement element)
            : this()
        {
            element.VerifyName(ElementNames.Page);
            Add(element.Attributes());
            Add(element.Elements(ElementNames.Field).Select(child => new XField(child)));
        }

        public Page Instantiate(View view)
        {
            return new Page(view)
            {
                Id = PageId,
                Name = Name,
                Position = Position,
                BackgroundId = BackgroundId
            };
        }

        public void Canonize(TemplateLevel level)
        {
            if (level >= TemplateLevel.Page)
            {
                BackgroundId = 0;
            }
            else
            {
                RemoveAttributes();
            }
        }
    }
}
