using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class Command : ICommand
    {
        public static event EventHandler<ErrorEventArgs> GlobalError;

        public static bool Always() => true;
        public static bool Always<T>(T parameter) => true;

        public static void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public Delegate Delegate { get; }
        public ErrorBehavior ErrorBehavior { get; }

        protected Command(Delegate @delegate, ErrorBehavior errorBehavior)
        {
            Delegate = @delegate;
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
            IEnumerable<Delegate> delegates = Enumerable.Concat(
                Error?.GetInvocationList() ?? Enumerable.Empty<Delegate>(),
                GlobalError?.GetInvocationList() ?? Enumerable.Empty<Delegate>());
            foreach (EventHandler<ErrorEventArgs> handler in delegates)
            {
                if (e.Handled)
                {
                    break;
                }
                handler(this, e);
            }
        }

        protected void OnError(Exception ex)
        {
            OnError(new ErrorEventArgs(ex));
        }

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
            return $"{Delegate.Method.DeclaringType}.{Delegate.Method.Name}";
        }
    }
}
