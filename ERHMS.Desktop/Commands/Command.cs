using ERHMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class Command : ICommand
    {
        private class NullCommand : Command
        {
            public NullCommand()
                : base("Null")
            {
                ErrorBehavior = ErrorBehavior.ThrowException;
            }

            public override bool CanExecute(object parameter)
            {
                return false;
            }

            public override Task ExecuteCore(object parameter)
            {
                throw new InvalidOperationException("Null command cannot be executed.");
            }
        }

        public static ICommand Null { get; } = new NullCommand();

        public static event EventHandler<ErrorEventArgs> GlobalError;

        protected static bool Always()
        {
            return true;
        }

        protected static bool Always<TParameter>(TParameter _)
        {
            return true;
        }

        public static void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public string Name { get; }
        public ErrorBehavior ErrorBehavior { get; set; } = ErrorBehavior.RaiseEvent;

        protected Command(string name)
        {
            Name = name;
        }

        protected Command(MethodInfo method)
            : this($"{method.DeclaringType.FullName}.{method.Name}") { }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void OnError(ErrorEventArgs e)
        {
            if (!e.Handled)
            {
                OnErrorCore(e, Error?.GetInvocationList());
                if (!e.Handled)
                {
                    OnErrorCore(e, GlobalError?.GetInvocationList());
                }
            }
        }

        private void OnErrorCore(ErrorEventArgs e, IEnumerable<Delegate> handlers)
        {
            if (handlers != null)
            {
                foreach (EventHandler<ErrorEventArgs> handler in handlers)
                {
                    handler(this, e);
                    if (e.Handled)
                    {
                        break;
                    }
                }
            }
        }

        protected void OnError(Exception exception) => OnError(new ErrorEventArgs(exception));

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteCore(object parameter);

        public async void Execute(object parameter)
        {
            Log.Instance.Debug($"Executing input command: {Name}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
                switch (ErrorBehavior)
                {
                    case ErrorBehavior.ThrowException:
                        throw;
                    case ErrorBehavior.RaiseEvent:
                        OnError(ex);
                        break;
                    case ErrorBehavior.Ignore:
                        break;
                    default:
                        throw;
                }
            }
            Log.Instance.Debug($"Executed input command: {Name}");
        }
    }
}
