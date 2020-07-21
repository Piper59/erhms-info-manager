using ERHMS.Desktop.Commands;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    public class SimpleSyncCommand : SyncCommand
    {
        public SimpleSyncCommand(Action execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }

    public class SimpleSyncCommand<T> : SyncCommand<T>
    {
        public SimpleSyncCommand(Action<T> execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }
}
