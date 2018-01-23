using ERHMS.Presentation.Services;
using System.Diagnostics;

namespace ERHMS.Presentation
{
    public class ProcessService : IProcessService
    {
        public Process Start(ProcessStartInfo info)
        {
            return Process.Start(info);
        }
    }
}
