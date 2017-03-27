namespace ERHMS.Domain
{
    public class DeepLink<TItem>
    {
        public TItem Item { get; private set; }
        public Incident Incident { get; private set; }

        public DeepLink(TItem item, Incident incident)
        {
            Item = item;
            Incident = incident;
        }
    }
}
