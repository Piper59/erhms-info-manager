using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class PgmLink : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("PgmLinkId");
            }
            set
            {
                if (!SetProperty("PgmLinkId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
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
    }
}
