namespace ERHMS.Presentation.Messages
{
    public class RefreshMessage<T>
    {
        public string IncidentId { get; private set; }

        public RefreshMessage(string incidentId = null)
        {
            IncidentId = incidentId;
        }
    }
}
