using Epi;
using ERHMS.EpiInfo.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XPage : XElement
    {
        public static XPage Create(Page page)
        {
            XPage xPage = new XPage
            {
                PageId = page.Id,
                Name = page.Name,
                Position = page.Position,
                BackgroundId = page.BackgroundId,
                ViewId = page.GetView().Id
            };
            xPage.OnCreated();
            return xPage;
        }

        public int PageId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int Position
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int BackgroundId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public int ViewId
        {
            get { return (int)this.GetAttributeEx(); }
            set { this.SetAttributeValueEx(value); }
        }

        public XView XView => (XView)Parent;
        public IEnumerable<XField> XFields => Elements().OfType<XField>();

        public XPage()
            : base(ElementNames.Page) { }

        public XPage(XElement element)
            : this()
        {
            Add(element.Attributes());
            foreach (XElement xField in element.Elements(ElementNames.Field))
            {
                Add(new XField(xField));
            }
            OnCreated();
        }

        private void OnCreated()
        {
            BackgroundId = 0;
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
    }
}
