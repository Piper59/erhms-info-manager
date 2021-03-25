using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class SyncCommand : Command
    {
        private readonly Action action;
        private readonly Func<bool> predicate;

        public SyncCommand(Action action, Func<bool> predicate = null)
            : base(action.Method)
        {
            this.action = action;
            this.predicate = predicate ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate();
        }

        public override Task ExecuteCore(object parameter)
        {
            action();
            return Task.CompletedTask;
        }
    }

    public class SyncCommand<TParameter> : Command
    {
        private readonly Action<TParameter> action;
        private readonly Func<TParameter, bool> predicate;

        public SyncCommand(Action<TParameter> action, Func<TParameter, bool> predicate = null)
            : base(action.Method)
        {
            this.action = action;
            this.predicate = predicate ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate((TParameter)parameter);
        }

        public override Task ExecuteCore(object parameter)
        {
            action((TParameter)parameter);
            return Task.CompletedTask;
        }
    }
}
