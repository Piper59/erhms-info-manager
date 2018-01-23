using ERHMS.Presentation.Services;
using System;
using System.Threading;

namespace ERHMS.Presentation
{
    public class DispatchService : IDispatchService
    {
        private SynchronizationContext context;

        public DispatchService(SynchronizationContext context)
        {
            this.context = context;
        }

        public void Post(Action action)
        {
            context.Post(state => action(), null);
        }

        public void Send(Action action)
        {
            context.Send(state => action(), null);
        }
    }
}
