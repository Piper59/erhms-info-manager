using System;

namespace ERHMS.EpiInfo.Domain
{
    public abstract class TableEntity : EntityBase
    {
        public abstract Guid? Id { get; set; }
    }
}
