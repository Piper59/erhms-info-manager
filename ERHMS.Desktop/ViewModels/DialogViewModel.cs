using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ObservableObject
    {
        public DialogInfo Info { get; }

        private bool isShowingDetails;
        public bool IsShowingDetails
        {
            get { return isShowingDetails; }
            set { SetProperty(ref isShowingDetails, value); }
        }

        public ICommand ToggleShowingDetailsCommand { get; }

        public DialogViewModel(DialogInfo info)
        {
            Info = info;
            ToggleShowingDetailsCommand = new SyncCommand(ToggleShowingDetails);
        }

        public void ToggleShowingDetails()
        {
            IsShowingDetails = !IsShowingDetails;
        }
    }
}
