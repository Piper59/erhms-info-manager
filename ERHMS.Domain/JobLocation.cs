using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class JobLocation : TableEntity
    {
        public override string Guid
        {
            get { return JobLocationId; }
            set { JobLocationId = value; }
        }

        public string JobLocationId
        {
            get { return GetProperty<string>(nameof(JobLocationId)); }
            set { SetProperty(nameof(JobLocationId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public string LocationId
        {
            get { return GetProperty<string>(nameof(LocationId)); }
            set { SetProperty(nameof(LocationId), value); }
        }

        public JobLocation()
        {
            AddSynonym(nameof(JobLocationId), nameof(Guid));
        }
    }
}
