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
            get { return GetProperty<string>("PgmLinkId"); }
            set { SetProperty("PgmLinkId", value); }
        }

        public int PgmId
        {
            get { return GetProperty<int>("PgmId"); }
            set { SetProperty("PgmId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }

        public PgmLink()
        {
            LinkProperties("PgmLinkId", "Guid");
        }
    }
}
