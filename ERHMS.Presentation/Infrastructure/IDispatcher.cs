using System;

namespace ERHMS.Presentation
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}
