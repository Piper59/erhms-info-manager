using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Roster : TableEntity
    {
        public override string Guid
        {
            get { return RosterId; }
            set { RosterId = value; }
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

        public Roster()
        {
            LinkProperties("RosterId", "Guid");
        }
    }
}
