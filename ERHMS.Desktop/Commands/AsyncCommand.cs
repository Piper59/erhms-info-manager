using System;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Commands
{
    public class AsyncCommand : Command
    {
        private readonly Func<Task> action;
        private readonly Func<bool> predicate;

        public AsyncCommand(Func<Task> action, Func<bool> predicate = null)
            : base(action)
        {
            this.action = action;
            this.predicate = predicate ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate();
        }

        public override async Task ExecuteCore(object parameter)
        {
            await action();
        }
    }

    public class AsyncCommand<TParameter> : Command
    {
        private readonly Func<TParameter, Task> action;
        private readonly Func<TParameter, bool> predicate;

        public AsyncCommand(Func<TParameter, Task> action, Func<TParameter, bool> predicate = null)
            : base(action)
        {
            this.action = action;
            this.predicate = predicate ?? Always;
        }

        public override bool CanExecute(object parameter)
        {
            return predicate((TParameter)parameter);
        }

        public override async Task ExecuteCore(object parameter)
        {
            await action((TParameter)parameter);
        }
    }
}
