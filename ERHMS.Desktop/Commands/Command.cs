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
        private class NullImpl : Command
        {
            public NullImpl()
                : base("Null") { }

            public override bool CanExecute(object parameter)
            {
                return false;
            }

            public override Task ExecuteCore(object parameter)
            {
                throw new NotSupportedException("The null command cannot be executed.");
            }
        }

        public static Command Null { get; } = new NullImpl();

        public static event EventHandler<ErrorEventArgs> GlobalError;

        protected static TParameter Cast<TParameter>(object parameter)
        {
            return parameter == null ? default(TParameter) : (TParameter)parameter;
        }

        public static void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        protected string Name { get; }
        public ErrorBehavior ErrorBehavior { get; set; } = ErrorBehavior.RaiseAndThrow;

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

        private void OnErrorCore(ErrorEventArgs e, IEnumerable<Delegate> handlers)
        {
            if (handlers == null)
            {
                return;
            }
            foreach (EventHandler<ErrorEventArgs> handler in handlers)
            {
                if (e.Handled)
                {
                    break;
                }
                handler(this, e);
            }
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            OnErrorCore(e, Error?.GetInvocationList());
            OnErrorCore(e, GlobalError?.GetInvocationList());
        }

        protected void OnError(Exception exception) => OnError(new ErrorEventArgs(exception));

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteCore(object parameter);

        public async void Execute(object parameter)
        {
            Log.Instance.Debug($"Executing input command: {this}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
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
                Log.Instance.Debug($"Executed input command: {this}");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
