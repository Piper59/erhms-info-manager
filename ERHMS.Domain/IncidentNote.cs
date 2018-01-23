using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class IncidentNote : IncidentEntity
    {
        public override string Guid
        {
            get { return IncidentNoteId; }
            set { IncidentNoteId = value; }
        }

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

        public IncidentNote(bool @new)
            : base(@new) { }

        public IncidentNote()
            : this(false) { }
    }
}
