using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        TimeSpan Delay { get; set; }
        string Title { get; set; }

        Task RunAsync(Action action);
        Task RunAsync(Func<Task> action);
        Task RunAsync(Action<CancellationToken> action);
        Task RunAsync(Func<CancellationToken, Task> action);
    }
}
