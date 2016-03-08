using System;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class TableEntity : EntityBase
    {
        public abstract Guid? Id { get; set; }
    }
}
