namespace ERHMS.EpiInfo.Domain
{
    public abstract class TableEntity : EntityBase
    {
        public abstract string Guid { get; set; }
    }
}
