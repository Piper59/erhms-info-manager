using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class SyncCommand : Command
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public SyncCommand(Action execute, Func<bool> canExecute = null)
            : base(execute.Method)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public override Task ExecuteCore(object parameter)
        {
            execute();
            return Task.CompletedTask;
        }
    }

    public class SyncCommand<TParameter> : Command
    {
        private readonly Action<TParameter> execute;
        private readonly Func<TParameter, bool> canExecute;

        public SyncCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute = null)
            : base(execute.Method)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute((TParameter)parameter);
        }

        public override Task ExecuteCore(object parameter)
        {
            execute((TParameter)parameter);
            return Task.CompletedTask;
        }
    }
}
