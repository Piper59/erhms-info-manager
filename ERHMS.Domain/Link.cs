using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public abstract class Link<TItem> : TableEntity
    {
        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public abstract bool IsEqual(TItem item);
    }
}
