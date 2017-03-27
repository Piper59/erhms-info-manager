using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Incident : TableEntity
    {
        public override string Guid
        {
            get { return IncidentId; }
            set { IncidentId = value; }
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

        public Phase Phase
        {
            get { return (Phase)Enum.Parse(typeof(Phase), GetProperty<string>(nameof(Phase))); }
            set { SetProperty(nameof(Phase), value.ToString()); }
        }

        public DateTime? StartDate
        {
            get { return GetProperty<DateTime?>(nameof(StartDate)); }
            set { SetProperty(nameof(StartDate), value); }
        }

        public DateTime? EndDateEstimate
        {
            get { return GetProperty<DateTime?>(nameof(EndDateEstimate)); }
            set { SetProperty(nameof(EndDateEstimate), value); }
        }

        public DateTime? EndDateActual
        {
            get { return GetProperty<DateTime?>(nameof(EndDateActual)); }
            set { SetProperty(nameof(EndDateActual), value); }
        }

        public bool Deleted
        {
            get { return GetProperty<bool>(nameof(Deleted)); }
            set { SetProperty(nameof(Deleted), value); }
        }

        public Incident()
        {
            AddSynonym(nameof(IncidentId), nameof(Guid));
        }
    }
}
