using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        string Title { get; set; }
        bool CanUserCancel { get; set; }
        bool IsUserCancellationRequested { get; }

        Task RunAsync(Action action, CancellationToken token);
        Task RunAsync(Action action);
    }
}
