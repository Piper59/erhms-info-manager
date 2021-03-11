using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        Task RunAsync(string title, Action action, Action continuation);
        Task RunAsync(string title, Action<CancellationToken> action, Action continuation);
    }

    public static class IProgressServiceExtensions
    {
        private static readonly Action NoOp = () => { };

        public static async Task RunAsync(this IProgressService @this, string title, Action action)
        {
            await @this.RunAsync(title, action, NoOp);
        }

        public static async Task RunAsync(this IProgressService @this, string title, Action<CancellationToken> action)
        {
            await @this.RunAsync(title, action, NoOp);
        }
    }
}
