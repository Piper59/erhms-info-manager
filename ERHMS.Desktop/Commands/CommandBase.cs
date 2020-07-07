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
            try
            {
                await ExecuteCore(parameter);
            }
            catch (Exception ex)
            {
                Log.Default.WarnFormat(
                    "{0} in {1}.{2}: {3}",
                    ex.GetType(),
                    execute.Target.GetType(),
                    execute.Method.Name,
                    ex.Message);
                throw;
            }
        }
    }
}
