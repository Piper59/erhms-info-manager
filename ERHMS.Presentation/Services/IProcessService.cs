using System.Diagnostics;

namespace ERHMS.Presentation.Services
{
    public interface IProcessService
    {
        Process Start(ProcessStartInfo info);
    }
}
