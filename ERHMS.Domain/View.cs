namespace ERHMS.Domain
{
    public class View : LinkedEntity<ViewLink>
    {
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
    }
}
