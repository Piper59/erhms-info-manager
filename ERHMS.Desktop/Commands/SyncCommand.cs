using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class SyncCommand : Command
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public SyncCommand(Action execute, Func<bool> canExecute, ErrorBehavior errorBehavior)
            : base(execute, errorBehavior)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => canExecute();

        public override Task ExecuteCore(object parameter)
        {
            execute();
            return Task.CompletedTask;
        }
    }

    public class SyncCommand<T> : Command
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public SyncCommand(Action<T> execute, Func<T, bool> canExecute, ErrorBehavior errorBehavior)
            : base(execute, errorBehavior)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => canExecute((T)parameter);

        public override Task ExecuteCore(object parameter)
        {
            execute((T)parameter);
            return Task.CompletedTask;
        }
    }
}
