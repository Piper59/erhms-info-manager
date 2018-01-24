using System;
using System.Windows.Input;

namespace ERHMS.Presentation.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected static string GetErrorMessage(Delegate execute)
        {
            return string.Format("Error in {0}: {1}", execute.Method.Name, execute.Target);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }
}
