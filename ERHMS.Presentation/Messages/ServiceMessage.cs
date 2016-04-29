using ERHMS.EpiInfo.Communication;

namespace ERHMS.Presentation.Messages
{
    public class ServiceMessage<TEventArgs> where TEventArgs : EventArgsBase
    {
        public string EventName { get; private set; }
        public TEventArgs Args { get; private set; }

        public ServiceMessage(string eventName, TEventArgs args)
        {
            EventName = eventName;
            Args = args;
        }
    }
}
