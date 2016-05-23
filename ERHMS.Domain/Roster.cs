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
            get { return GetProperty<string>(nameof(RosterId)); }
            set { SetProperty(nameof(RosterId), value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public Roster()
        {
            LinkProperties(nameof(RosterId), nameof(Guid));
        }
    }
}
