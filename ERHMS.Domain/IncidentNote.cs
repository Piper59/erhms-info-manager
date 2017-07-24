using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class IncidentNote : Entity
    {
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

        private Incident incident;
        public Incident Incident
        {
            get { return incident; }
            set { SetProperty(nameof(Incident), ref incident, value); }
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
    }
}
