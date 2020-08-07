using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : Command
    {
        private readonly Func<Task> executeAsync;
        private readonly Func<bool> canExecute;

        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute, ErrorBehavior errorBehavior)
            : base(executeAsync, errorBehavior)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => canExecute();
        public override async Task ExecuteCore(object parameter) => await executeAsync();
    }

    public class AsyncCommand<T> : Command
    {
        private readonly Func<T, Task> executeAsync;
        private readonly Func<T, bool> canExecute;

        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute, ErrorBehavior errorBehavior)
            : base(executeAsync, errorBehavior)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => canExecute((T)parameter);
        public override async Task ExecuteCore(object parameter) => await executeAsync((T)parameter);
    }
}
