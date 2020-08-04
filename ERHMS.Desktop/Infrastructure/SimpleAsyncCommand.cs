using ERHMS.Desktop.Commands;
using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Infrastructure
{
    public class SimpleAsyncCommand : AsyncCommand
    {
        public SimpleAsyncCommand(Func<Task> executeAsync)
            : base(executeAsync, Always, ErrorBehavior.Raise) { }
    }

    public class SimpleAsyncCommand<T> : AsyncCommand<T>
    {
        public SimpleAsyncCommand(Func<T, Task> executeAsync)
            : base(executeAsync, Always, ErrorBehavior.Raise) { }
    }
}
