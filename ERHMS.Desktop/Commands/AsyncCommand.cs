using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : Command
    {
        private readonly Func<Task> executeAsync;
        private readonly Func<bool> canExecute;

        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
            : base(executeAsync.Method)
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

    public class AsyncCommand<TParameter> : Command
    {
        private readonly Func<TParameter, Task> executeAsync;
        private readonly Func<TParameter, bool> canExecute;

        public AsyncCommand(Func<TParameter, Task> executeAsync, Func<TParameter, bool> canExecute = null)
            : base(executeAsync.Method)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute((TParameter)parameter);
        }

        public override async Task ExecuteCore(object parameter)
        {
            await executeAsync((TParameter)parameter);
        }
    }
}
