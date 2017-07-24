using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class IncidentNote : Link
    {
        public string IncidentNoteId
        {
            get { return GetProperty<string>(nameof(IncidentNoteId)); }
            set { SetProperty(nameof(IncidentNoteId), value); }
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

        public IncidentNote()
        {
            IncidentNoteId = Guid.NewGuid().ToString();
        }
    }
}
