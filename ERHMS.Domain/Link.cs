namespace ERHMS.Domain
{
    public class Link<T>
    {
        public T Data { get; private set; }
        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        public string IncidentName
        {
            get { return Incident == null ? null : Incident.Name; }
        }

        public Link(T data, Incident incident)
        {
            Data = data;
            Incident = incident;
        }
    }
}
