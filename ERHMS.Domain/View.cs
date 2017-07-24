using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class View : Entity
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

        private ViewLink viewLink;
        public ViewLink ViewLink
        {
            get { return viewLink; }
            set { SetProperty(nameof(ViewLink), ref viewLink, value); }
        }

        public Incident Incident
        {
            get { return ViewLink?.Incident; }
        }
    }
}
