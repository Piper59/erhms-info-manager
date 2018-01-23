using System;
using System.Threading.Tasks;

namespace ERHMS.Presentation.Commands
{
    public class AsyncCommand : CommandBase
    {
        private Func<Task> executeAsync;
        private Func<bool> canExecute;

        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? (() => true);
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public async override void Execute(object parameter)
        {
            await executeAsync();
        }
    }

    public class AsyncCommand<T> : CommandBase
    {
        private Func<T, Task> executeAsync;
        private Func<T, bool> canExecute;

        public AsyncCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute = null)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? (parameter => true);
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public async override void Execute(object parameter)
        {
            await executeAsync((T)parameter);
        }
    }
}
