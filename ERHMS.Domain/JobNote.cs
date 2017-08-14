using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class JobNote : Entity
    {
        public string JobNoteId
        {
            get { return GetProperty<string>(nameof(JobNoteId)); }
            set { SetProperty(nameof(JobNoteId), value); }
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

        public string Content
        {
            get { return GetProperty<string>(nameof(Content)); }
            set { SetProperty(nameof(Content), value); }
        }

        public DateTime Date
        {
            get { return GetProperty<DateTime>(nameof(Date)); }
            set { SetProperty(nameof(Date), value.RemoveMilliseconds()); }
        }

        public JobNote()
        {
            JobNoteId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            JobNote clone = (JobNote)base.Clone();
            clone.Job = (Job)Job.Clone();
            return clone;
        }
    }
}
