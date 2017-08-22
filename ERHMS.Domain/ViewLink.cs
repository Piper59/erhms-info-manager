namespace ERHMS.Domain
{
    public class ViewLink : Link
    {
        protected override string Guid
        {
            get { return ViewLinkId; }
            set { ViewLinkId = value; }
        }

        public string ViewLinkId
        {
            get { return GetProperty<string>(nameof(ViewLinkId)); }
            set { SetProperty(nameof(ViewLinkId), value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>(nameof(ViewId)); }
            set { SetProperty(nameof(ViewId), value); }
        }

        public ViewLink(bool @new)
            : base(@new) { }

        public ViewLink()
            : this(false) { }
    }
}
