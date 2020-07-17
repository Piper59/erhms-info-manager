using ERHMS.Desktop.Commands;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    internal class SimpleCommand : Command
    {
        public SimpleCommand(Action execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }

    internal class SimpleCommand<T> : Command<T>
    {
        public SimpleCommand(Action<T> execute)
            : base(execute, Always, ErrorBehavior.Raise) { }
    }
}
