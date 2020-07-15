using Epi;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XView : XElement
    {
        public static XView Construct(View view)
        {
            XView xView = new XView
            {
                ViewId = view.Id,
                Name = view.Name,
                IsRelatedView = view.IsRelatedView,
                CheckCode = view.CheckCode,
                Width = view.PageWidth,
                Height = view.PageHeight,
                Orientation = view.PageOrientation,
                LabelAlign = view.PageLabelAlign,
                SurveyId = ConfigurationExtensions.CompatibilityMode ? view.WebSurveyId : null
            };
            foreach (Page page in view.Pages)
            {
                xView.Add(XPage.Construct(page));
            }
            return xView;
        }

        public static XView Construct(Page page)
        {
            XView xView = new XView
            {
                CheckCode = page.GetView().CheckCode
            };
            xView.Add(XPage.Construct(page));
            return xView;
        }

        public static XView Wrap(XElement element)
        {
            XView xView = new XView();
            xView.Add(element.Attributes());
            foreach (XElement xPage in element.Elements(ElementNames.Page))
            {
                xView.Add(XPage.Wrap(xPage));
            }
            return xView;
        }

        public int ViewId
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public bool IsRelatedView
        {
            get { return (bool)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string CheckCode
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public int Width
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public int Height
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string Orientation
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string LabelAlign
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public string SurveyId
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        private XView()
            : base(ElementNames.View) { }
    }
}
