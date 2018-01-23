using System;

namespace ERHMS.Presentation.Services
{
    public interface IDispatchService
    {
        void Post(Action action);
        void Send(Action action);
    }
}
