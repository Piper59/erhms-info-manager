using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class JobLocation : GuidEntity
    {
        protected override string Guid
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

        private Job job;
        public Job Job
        {
            get { return job; }
            set { SetProperty(nameof(Job), ref job, value); }
        }

        public string LocationId
        {
            get { return GetProperty<string>(nameof(LocationId)); }
            set { SetProperty(nameof(LocationId), value); }
        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set { SetProperty(nameof(Location), ref location, value); }
        }

        public JobLocation(bool @new)
            : base(@new) { }

        public JobLocation()
            : this(false) { }

        public override object Clone()
        {
            JobLocation clone = (JobLocation)base.Clone();
            clone.Job = (Job)Job.Clone();
            clone.Location = (Location)Location.Clone();
            return clone;
        }
    }
}
