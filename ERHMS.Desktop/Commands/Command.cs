using ERHMS.Desktop.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class Command : ICommand
    {
        public static event EventHandler<ErrorEventArgs> GlobalError;

        public static void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public static bool Always()
        {
            return true;
        }

        public static bool Always<T>(T parameter)
        {
            return true;
        }

        public string Name { get; }
        public ErrorBehavior ErrorBehavior { get; }

        protected Command(Delegate execute, ErrorBehavior errorBehavior)
        {
            Name = $"{execute.Method.DeclaringType}.{execute.Method.Name}";
            ErrorBehavior = errorBehavior;
        }

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
