using System;

namespace ERHMS.Presentation.Services
{
    public interface IDispatcherService
    {
        void Post(Action action);
        void Send(Action action);
    }
}
