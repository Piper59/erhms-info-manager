using ERHMS.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class Command : ICommand
    {
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

        protected Delegate Action { get; }
        public ErrorBehavior ErrorBehavior { get; set; } = ErrorBehavior.RaiseAndThrow;

        protected Command(Delegate action)
        {
            Action = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public event EventHandler<ErrorEventArgs> Error;

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

        protected void OnError(Exception exception) => OnError(new ErrorEventArgs(exception));

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteCore(object parameter);

        public async void Execute(object parameter)
        {
            Log.Default.Debug($"Executing input command: {this}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Default.Warn(ex);
                switch (ErrorBehavior)
                {
                    case ErrorBehavior.Throw:
                        throw;
                    case ErrorBehavior.RaiseAndThrow:
                        ErrorEventArgs e = new ErrorEventArgs(ex);
                        OnError(e);
                        if (!e.Handled)
                        {
                            throw;
                        }
                        break;
                    case ErrorBehavior.RaiseAndIgnore:
                        OnError(ex);
                        break;
                    case ErrorBehavior.Ignore:
                        break;
                    default:
                        throw;
                }
            }
            finally
            {
                Log.Default.Debug($"Executed input command: {this}");
            }
        }

        public override string ToString()
        {
            return $"{Action.Method.DeclaringType.FullName}.{Action.Method.Name}";
        }
    }
}
