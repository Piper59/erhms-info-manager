using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class PgmLink : Entity
    {
        public string PgmLinkId
        {
            get { return GetProperty<string>(nameof(PgmLinkId)); }
            set { SetProperty(nameof(PgmLinkId), value); }
        }

        public int PgmId
        {
            get { return GetProperty<int>(nameof(PgmId)); }
            set { SetProperty(nameof(PgmId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        private Incident incident;
        public Incident Incident
        {
            get { return incident; }
            set { SetProperty(nameof(Incident), ref incident, value); }
        }
    }
}
