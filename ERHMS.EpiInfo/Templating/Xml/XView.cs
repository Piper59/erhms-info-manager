using Epi;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public class XView : XElement
    {
        public static XView Create(View view)
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
                SurveyId = view.WebSurveyId
            };
            return xView;
        }

        public int ViewId
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public bool IsRelatedView
        {
            get { return (bool)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string CheckCode
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int Width
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public int Height
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string Orientation
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string LabelAlign
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public string SurveyId
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetOrClearAttributeValue(value); }
        }

        public XProject XProject => (XProject)Parent;
        public IEnumerable<XPage> XPages => Elements(ElementNames.Page).Cast<XPage>();
        public IEnumerable<XField> XFields => XPages.SelectMany(xPage => xPage.XFields);

        public XView()
            : base(ElementNames.View) { }

        public XView(XElement element)
            : this()
        {
            Add(element.Attributes());
            Add(element.Elements(ElementNames.Page).Select(child => new XPage(child)));
        }

        public View Instantiate(Project project)
        {
            return new View(project)
            {
                Id = ViewId,
                Name = Name,
                IsRelatedView = IsRelatedView,
                CheckCode = CheckCode,
                PageWidth = Width,
                PageHeight = Height,
                PageOrientation = Orientation,
                PageLabelAlign = LabelAlign
            };
        }

        public void Unrelate()
        {
            if (Attribute(nameof(IsRelatedView)) != null)
            {
                IsRelatedView = false;
            }
            IReadOnlyCollection<XField> relateXFields =
                XFields.Where(xField => xField.FieldType == MetaFieldType.Relate).ToList();
            foreach (XField relateXField in relateXFields)
            {
                relateXField.Remove();
            }
        }
    }
}
