using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        TimeSpan FeedbackDelay { get; set; }

        Task RunAsync(string title, Action action);
        Task RunAsync(string title, Func<Task> action);
        Task RunAsync(string title, Action<CancellationToken> action);
        Task RunAsync(string title, Func<CancellationToken, Task> action);
    }
}
