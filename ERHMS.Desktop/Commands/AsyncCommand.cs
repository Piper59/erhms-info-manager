using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : Command
    {
        private readonly Func<Task> executeAsync;
        private readonly Func<bool> canExecute;

        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute = null, ErrorBehavior errorBehavior = ErrorBehavior.Raise)
            : base(executeAsync, errorBehavior)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public override async Task ExecuteCore(object parameter)
        {
            await executeAsync();
        }
    }

    public class AsyncCommand<T> : Command
    {
        private readonly Func<T, Task> executeAsync;
        private readonly Func<T, bool> canExecute;

        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute = null, ErrorBehavior errorBehavior = ErrorBehavior.Raise)
            : base(executeAsync, errorBehavior)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public override async Task ExecuteCore(object parameter)
        {
            await executeAsync((T)parameter);
        }
    }
}
