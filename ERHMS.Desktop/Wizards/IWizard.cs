using System;
using System.ComponentModel;
using System.Windows.Input;

namespace ERHMS.Desktop.Wizards
{
    public interface IWizard : INotifyPropertyChanged
    {
        IStep CurrentStep { get; }
        bool? Result { get; }
        bool Completed { get; }

        ICommand ExitCommand { get; }

        event EventHandler ExitRequested;
    }
}
