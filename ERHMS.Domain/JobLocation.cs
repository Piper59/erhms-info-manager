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

        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
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
