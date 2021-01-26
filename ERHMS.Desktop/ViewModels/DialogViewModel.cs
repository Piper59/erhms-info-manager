using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ObservableObject
    {
        public Dialog Dialog { get; }

        private bool showingDetails;
        public bool ShowingDetails
        {
            get { return showingDetails; }
            set { SetProperty(ref showingDetails, value); }
        }

        public ICommand ToggleShowingDetailsCommand { get; }

        public DialogViewModel(Dialog dialog)
        {
            Dialog = dialog;
            ToggleShowingDetailsCommand = new SyncCommand(ToggleShowingDetails);
        }

        public void ToggleShowingDetails()
        {
            ShowingDetails = !ShowingDetails;
        }
    }
}
