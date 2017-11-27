using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public abstract class EpiInfoEntity<TLink> : Entity
        where TLink : IncidentEntity
    {
        public abstract TLink Link { get; set; }

        public Incident Incident
        {
            get { return Link?.Incident; }
        }

        protected EpiInfoEntity(bool @new)
            : base(@new) { }

        public override object Clone()
        {
            EpiInfoEntity<TLink> clone = (EpiInfoEntity<TLink>)base.Clone();
            clone.Link = (TLink)Link.Clone();
            return clone;
        }
    }
}
