using ERHMS.Presentation.Services;
using System;
using System.Threading;

namespace ERHMS.Presentation
{
    public class DispatcherService : IDispatcherService
    {
        private SynchronizationContext synchronizationContext;

        public DispatcherService()
        {
            synchronizationContext = SynchronizationContext.Current;
        }

        public void Post(Action action)
        {
            synchronizationContext.Post(state => action(), null);
        }

        public void Send(Action action)
        {
            synchronizationContext.Send(state => action(), null);
        }
    }
}
