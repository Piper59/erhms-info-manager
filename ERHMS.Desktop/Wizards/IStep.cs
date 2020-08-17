using System.ComponentModel;
using System.Windows.Input;

namespace ERHMS.Desktop.Wizards
{
    public interface IStep : INotifyPropertyChanged
    {
        IStep Previous { get; }
        bool CanReturn { get; }
        string ContinueText { get; }

        ICommand ReturnCommand { get; }
        ICommand ContinueCommand { get; }
    }
}
