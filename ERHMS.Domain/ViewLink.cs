using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class ViewLink : TableEntity
    {
        public override string Guid
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

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public ViewLink()
        {
            LinkProperties(nameof(ViewLinkId), nameof(Guid));
        }
    }
}
