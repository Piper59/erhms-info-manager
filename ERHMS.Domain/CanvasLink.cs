using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class CanvasLink : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("CanvasLinkId");
            }
            set
            {
                if (!SetProperty("CanvasLinkId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string CanvasLinkId
        {
            get { return GetProperty<string>("CanvasLinkId"); }
            set { SetProperty("CanvasLinkId", value); }
        }

        public int CanvasId
        {
            get { return GetProperty<int>("CanvasId"); }
            set { SetProperty("CanvasId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
