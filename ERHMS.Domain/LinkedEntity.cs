using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public abstract class LinkedEntity<TLink> : Entity
        where TLink : Link
    {
        public abstract TLink Link { get; set; }

        public Incident Incident
        {
            get { return Link?.Incident; }
        }

        protected LinkedEntity(bool @new)
            : base(@new) { }

        public override object Clone()
        {
            LinkedEntity<TLink> clone = (LinkedEntity<TLink>)base.Clone();
            clone.Link = (TLink)Link.Clone();
            return clone;
        }
    }
}
