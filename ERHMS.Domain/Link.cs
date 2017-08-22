using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public abstract class Link : GuidEntity
    {
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

        protected Link(bool @new)
            : base(@new) { }

        public override object Clone()
        {
            Link clone = (Link)base.Clone();
            clone.Incident = (Incident)Incident.Clone();
            return clone;
        }
    }
}
