using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Roster : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("RosterId");
            }
            set
            {
                if (!SetProperty("RosterId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string RosterId
        {
            get { return GetProperty<string>("RosterId"); }
            set { SetProperty("RosterId", value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>("ResponderId"); }
            set { SetProperty("ResponderId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
