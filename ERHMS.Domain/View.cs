namespace ERHMS.Domain
{
    public class View : EpiInfoEntity<ViewLink>
    {
        public override object Id
        {
            get { return ViewId; }
        }

        public int ViewId
        {
            get { return GetProperty<int>(nameof(ViewId)); }
            set { SetProperty(nameof(ViewId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public string WebSurveyId
        {
            get { return GetProperty<string>(nameof(WebSurveyId)); }
            set { SetProperty(nameof(WebSurveyId), value); }
        }

        public bool HasResponderIdField
        {
            get { return GetProperty<bool>(nameof(HasResponderIdField)); }
            set { SetProperty(nameof(HasResponderIdField), value); }
        }

        public string ViewLinkId
        {
            get { return GetProperty<string>(nameof(ViewLinkId)); }
            set { SetProperty(nameof(ViewLinkId), value); }
        }

        private ViewLink link;
        public override ViewLink Link
        {
            get { return link; }
            set { SetProperty(nameof(Link), ref link, value); }
        }

        public View(bool @new)
            : base(@new) { }

        public View()
            : this(false) { }
    }
}
