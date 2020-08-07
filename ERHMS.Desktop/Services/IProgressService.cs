using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        Task RunAsync(Action action, CancellationToken token);
        Task RunAsync(Action action);
    }
}
