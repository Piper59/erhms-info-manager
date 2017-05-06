using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Job : TableEntity
    {
        public override string Guid
        {
            get { return JobId; }
            set { JobId = value; }
        }

        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public string Description
        {
            get { return GetProperty<string>(nameof(Description)); }
            set { SetProperty(nameof(Description), value); }
        }

        public DateTime StartDate
        {
            get { return GetProperty<DateTime>(nameof(StartDate)); }
            set { SetProperty(nameof(StartDate), value); }
        }

        public DateTime EndDate
        {
            get { return GetProperty<DateTime>(nameof(EndDate)); }
            set { SetProperty(nameof(EndDate), value); }
        }

        public Job()
        {
            AddSynonym(nameof(JobId), nameof(Guid));
        }
    }
}
