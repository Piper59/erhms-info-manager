using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public interface IUtility
    {
        bool LongRunning { get; }
        IProgress<string> Progress { get; set; }

        Task RunAsync();
    }
}
