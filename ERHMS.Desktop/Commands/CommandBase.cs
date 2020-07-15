using log4net;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected static ILog Log { get; } = LogManager.GetLogger(nameof(ERHMS));

        private Delegate execute;

        protected CommandBase(Delegate execute)
        {
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteCore(object parameter);

        public async void Execute(object parameter)
        {
            Log.Debug($"Executing: {this}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Warn($"{ex.GetType()} in {this}: {ex.Message}");
                // TODO: Recover by default?
                throw;
            }
            Log.Debug($"Executed: {this}");
        }

        public override string ToString()
        {
            return $"{execute.Method.DeclaringType}.{execute.Method.Name}";
        }
    }
}
