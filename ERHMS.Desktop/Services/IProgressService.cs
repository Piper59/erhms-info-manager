using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        string Lead { get; set; }
        bool CanBeCanceled { get; set; }
        CancellationToken CancellationToken { get; }
        TimeSpan Delay { get; set; }

        Task Run(Func<Task> action);
        Task<TResult> Run<TResult>(Func<Task<TResult>> action);
    }

    public static class IProgressServiceExtensions
    {
        public static Task Run(this IProgressService @this, Action action)
        {
            return @this.Run(() => Task.Run(action));
        }

        public static Task<TResult> Run<TResult>(this IProgressService @this, Func<TResult> action)
        {
            return @this.Run(() => Task.Run(action));
        }
    }
}
