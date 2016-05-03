namespace ERHMS.Presentation.Messages
{
    public class RefreshListMessage<T>
    {
        public string IncidentId { get; private set; }

        public RefreshListMessage(string incidentId = null)
        {
            IncidentId = incidentId;
        }
    }
}
