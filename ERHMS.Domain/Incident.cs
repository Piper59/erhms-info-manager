using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;

namespace ERHMS.Domain
{
    public class Incident : GuidEntity
    {
        protected override string Guid
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
            get { return EnumExtensions.Parse<Phase>(GetProperty<string>(nameof(Phase))); }
#pragma warning disable 618
            set { SetProperty(nameof(Phase), (value == Phase.Closed ? Phase.PostDeployment : value).ToString()); }
#pragma warning restore 618
        }

        public DateTime? StartDate
        {
            get { return GetProperty<DateTime?>(nameof(StartDate)); }
            set { SetProperty(nameof(StartDate), value?.RemoveMilliseconds()); }
        }

        public DateTime? EndDateEstimate
        {
            get { return GetProperty<DateTime?>(nameof(EndDateEstimate)); }
            set { SetProperty(nameof(EndDateEstimate), value?.RemoveMilliseconds()); }
        }

        public DateTime? EndDateActual
        {
            get { return GetProperty<DateTime?>(nameof(EndDateActual)); }
            set { SetProperty(nameof(EndDateActual), value?.RemoveMilliseconds()); }
        }

        public bool Deleted
        {
            get { return GetProperty<bool>(nameof(Deleted)); }
            set { SetProperty(nameof(Deleted), value); }
        }

        public Incident(bool @new)
            : base(@new)
        {
            Phase = Phase.PreDeployment;
            Deleted = false;
        }

        public Incident()
            : this(false) { }
    }
}
