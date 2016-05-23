using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class CanvasLink : TableEntity
    {
        public override string Guid
        {
            get { return CanvasLinkId; }
            set { CanvasLinkId = value; }
        }

        public string CanvasLinkId
        {
            get { return GetProperty<string>(nameof(CanvasLinkId)); }
            set { SetProperty(nameof(CanvasLinkId), value); }
        }

        public int CanvasId
        {
            get { return GetProperty<int>(nameof(CanvasId)); }
            set { SetProperty(nameof(CanvasId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public CanvasLink()
        {
            LinkProperties(nameof(CanvasLinkId), nameof(Guid));
        }
    }
}
