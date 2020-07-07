using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class Command : CommandBase
    {
        private Action execute;
        private Func<bool> canExecute;

        public Command(Action execute, Func<bool> canExecute = null)
            : base(execute)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (() => true);
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

    public class Command<T> : CommandBase
    {
        private Action<T> execute;
        private Func<T, bool> canExecute;

        public Command(Action<T> execute, Func<T, bool> canExecute = null)
            : base(execute)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (parameter => true);
        }

        public override bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public override Task ExecuteCore(object parameter)
        {
            execute((T)parameter);
            return Task.CompletedTask;
        }
    }
}
