using Epi;
using ERHMS.Utility;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XView : XElement
    {
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

        public XView(View view)
            : this()
        {
            Log.Default.Debug($"Adding view: {view.Name}");
            ViewId = view.Id;
            Name = view.Name;
            IsRelatedView = view.IsRelatedView;
            CheckCode = view.CheckCode;
            Width = view.PageWidth;
            Height = view.PageHeight;
            Orientation = view.PageOrientation;
            LabelAlign = view.PageLabelAlign;
            SurveyId = ConfigurationExtensions.CompatibilityMode ? view.WebSurveyId : null;
            foreach (Page page in view.Pages)
            {
                Add(new XPage(page));
            }
        }

        public XView(Page page)
            : this()
        {
            CheckCode = page.GetView().CheckCode;
            Add(new XPage(page));
        }

        public XView(XElement element)
            : base(ElementNames.View)
        {
            Add(element.Attributes());
            foreach (XElement xPage in element.Elements(ElementNames.Page))
            {
                Add(new XPage(xPage));
            }
        }
    }
}
