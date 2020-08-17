using ERHMS.Desktop.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class Command : ICommand
    {
        private class NullCommand : Command
        {
            public NullCommand()
                : base("Null", ErrorBehavior.Throw) { }

            public override bool CanExecute(object parameter)
            {
                return false;
            }

            public override Task ExecuteCore(object parameter)
            {
                throw new InvalidOperationException("The null command cannot be executed.");
            }
        }

        public static readonly ICommand Null = new NullCommand();

        public static event EventHandler<ErrorEventArgs> GlobalError;

        public static void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        protected static bool Always() => true;
        protected static bool Always<T>(T parameter) => true;

        public string Name { get; }
        public ErrorBehavior ErrorBehavior { get; }

        protected Command(string name, ErrorBehavior errorBehavior)
        {
            Name = name;
            ErrorBehavior = errorBehavior;
        }

        protected Command(Delegate execute, ErrorBehavior errorBehavior)
            : this($"{execute.Method.DeclaringType}.{execute.Method.Name}", errorBehavior) { }

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
                OnErrorInternal(e, Error?.GetInvocationList());
                if (!e.Handled)
                {
                    OnErrorInternal(e, GlobalError?.GetInvocationList());
                }
            }
        }

        private void OnErrorInternal(ErrorEventArgs e, IEnumerable<Delegate> handlers)
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

        protected void OnError(Exception ex) => OnError(new ErrorEventArgs(ex));

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteCore(object parameter);

        public async void Execute(object parameter)
        {
            Log.Default.Debug($"Executing: {this}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Default.Warn(ex);
                switch (ErrorBehavior)
                {
                    case ErrorBehavior.Catch:
                        break;
                    case ErrorBehavior.Raise:
                        OnError(ex);
                        break;
                    case ErrorBehavior.Throw:
                    default:
                        throw;
                }
            }
            Log.Default.Debug($"Executed: {this}");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
