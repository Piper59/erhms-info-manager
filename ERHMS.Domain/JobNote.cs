using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class JobNote : TableEntity
    {
        public override string Guid
        {
            get { return JobNoteId; }
            set { JobNoteId = value; }
        }

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

        public string Content
        {
            get { return GetProperty<string>(nameof(Content)); }
            set { SetProperty(nameof(Content), value); }
        }

        public DateTime Date
        {
            get { return GetProperty<DateTime>(nameof(Date)); }
            set { SetProperty(nameof(Date), value); }
        }

        public JobNote()
        {
            AddSynonym(nameof(JobNoteId), nameof(Guid));
        }
    }
}
