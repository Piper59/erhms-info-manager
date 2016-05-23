using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class PgmLink : TableEntity
    {
        public override string Guid
        {
            get { return PgmLinkId; }
            set { PgmLinkId = value; }
        }

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

        public PgmLink()
        {
            LinkProperties(nameof(PgmLinkId), nameof(Guid));
        }
    }
}
