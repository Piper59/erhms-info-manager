using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Utility
{
    public static class ThreadingExtensions
    {
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }
    }
}
