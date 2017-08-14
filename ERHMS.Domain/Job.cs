using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class Job : Link
    {
        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
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

        public DateTime? StartDate
        {
            get { return GetProperty<DateTime?>(nameof(StartDate)); }
            set { SetProperty(nameof(StartDate), value?.RemoveMilliseconds()); }
        }

        public DateTime? EndDate
        {
            get { return GetProperty<DateTime?>(nameof(EndDate)); }
            set { SetProperty(nameof(EndDate), value?.RemoveMilliseconds()); }
        }

        public Job()
        {
            JobId = Guid.NewGuid().ToString();
        }
    }
}
