using ERHMS.Common;
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
                : base(nameof(Null)) { }

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
