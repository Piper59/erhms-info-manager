using System;

namespace ERHMS.EpiInfo.Data
{
    public abstract class TableEntity : EntityBase
    {
        public abstract Guid? Id { get; set; }
    }
}
