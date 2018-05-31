using ERHMS.Presentation.Services;
using System;

namespace ERHMS.Presentation
{
    public class BusyService : IBusyService
    {
        public IDisposable Begin()
        {
            return new WaitCursor();
        }
    }
}
