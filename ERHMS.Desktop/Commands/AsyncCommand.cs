using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : Command
    {
        private readonly Func<Task> action;
        private readonly Func<bool> predicate;

        public AsyncCommand(Func<Task> action, Func<bool> predicate = null)
            : base(action.Method)
        {
            this.action = action;
            this.predicate = predicate;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate == null || predicate();
        }

        public override Task ExecuteCore(object parameter)
        {
            return action();
        }
    }

    public class AsyncCommand<TParameter> : Command
    {
        private readonly Func<TParameter, Task> action;
        private readonly Func<TParameter, bool> predicate;

        public AsyncCommand(Func<TParameter, Task> action, Func<TParameter, bool> predicate = null)
            : base(action.Method)
        {
            this.action = action;
            this.predicate = predicate;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate == null || predicate((TParameter)parameter);
        }

        public override Task ExecuteCore(object parameter)
        {
            return action((TParameter)parameter);
        }
    }
}
