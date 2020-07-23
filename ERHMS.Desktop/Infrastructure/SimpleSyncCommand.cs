using ERHMS.Desktop.Commands;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    internal class SimpleSyncCommand : SyncCommand
    {
        public SimpleSyncCommand(Action execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }

    internal class SimpleSyncCommand<T> : SyncCommand<T>
    {
        public SimpleSyncCommand(Action<T> execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }
}
