using System;

namespace ERHMS.Presentation.Services
{
    public interface IBusyService
    {
        IDisposable Begin();
    }
}
