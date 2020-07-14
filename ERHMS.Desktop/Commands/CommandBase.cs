using ERHMS.Utility;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.Commands
{
    public abstract class CommandBase : ICommand
    {
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
            Log.Default.Debug($"Executing: {this}");
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Default.Warn($"{ex.GetType()} in {this}: {ex.Message}");
                // TODO: Recover by default?
                throw;
            }
            Log.Default.Debug($"Executed: {this}");
        }

        public override string ToString()
        {
            return $"{execute.Method.DeclaringType}.{execute.Method.Name}";
        }
    }
}
