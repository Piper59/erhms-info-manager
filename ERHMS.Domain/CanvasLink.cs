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
            get { return GetProperty<string>("CanvasLinkId"); }
            set { SetProperty("CanvasLinkId", value); }
        }

        public string Path
        {
            get { return GetProperty<string>("Path"); }
            set { SetProperty("Path", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
