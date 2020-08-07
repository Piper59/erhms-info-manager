﻿using Epi;
using ERHMS.EpiInfo.Infrastructure;
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
            set { this.SetAttributeValue(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public bool IsRelatedView
        {
            get { return (bool)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string CheckCode
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int Width
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public int Height
        {
            get { return (int)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string Orientation
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string LabelAlign
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string SurveyId
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public XProject XProject => (XProject)Parent;
        public IEnumerable<XPage> XPages => Elements().OfType<XPage>();
        public IEnumerable<XField> XFields => Descendants().OfType<XField>();

        public XView()
            : base(ElementNames.View) { }

        public XView(XElement element)
            : this()
        {
            Add(element.Attributes());
            foreach (XElement xPage in element.Elements(ElementNames.Page))
            {
                Add(new XPage(xPage));
            }
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
    }
}