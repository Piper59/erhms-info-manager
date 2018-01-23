using System;

namespace ERHMS.EpiInfo.Domain
{
    public abstract class GuidEntity : Entity
    {
        public override object Id
        {
            get { return Guid; }
        }

        public abstract string Guid { get; set; }

        protected GuidEntity(bool @new)
            : base(@new)
        {
            if (@new)
            {
                Guid = System.Guid.NewGuid().ToString();
            }
        }

        public override int GetHashCode()
        {
            return Guid == null ? 0 : Guid.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            GuidEntity entity = obj as GuidEntity;
            return entity != null && entity.Guid != null && entity.Guid.Equals(Guid, StringComparison.OrdinalIgnoreCase);
        }
    }
}
