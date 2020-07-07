using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : CommandBase
    {
        private Func<Task> executeAsync;
        private Func<bool> canExecute;

        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
            : base(executeAsync)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? (() => true);
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

    public class AsyncCommand<T> : CommandBase
    {
        private Func<T, Task> executeAsync;
        private Func<T, bool> canExecute;

        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute = null)
            : base(executeAsync)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? (parameter => true);
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
