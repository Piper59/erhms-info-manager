using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class IncidentNote : TableEntity
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

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
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

        public IncidentNote()
        {
            LinkProperties(nameof(IncidentNoteId), nameof(Guid));
        }
    }
}
