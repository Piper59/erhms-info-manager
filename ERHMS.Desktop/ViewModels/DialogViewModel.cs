using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure.ViewModels;
using System.Drawing;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ViewModel
    {
        public DialogType DialogType { get; }
        public Icon Icon { get; }
        public string Lead { get; }
        public string Body { get; }
        public string Details { get; }
        public DialogButtonCollection Buttons { get; }

        private bool showingDetails;
        public bool ShowingDetails
        {
            get { return showingDetails; }
            set { SetProperty(ref showingDetails, value); }
        }

        public ICommand ToggleShowingDetailsCommand { get; }

        public DialogViewModel(
            DialogType dialogType,
            string lead,
            string body,
            string details,
            DialogButtonCollection buttons)
        {
            DialogType = dialogType;
            Icon = dialogType.ToIcon();
            Lead = lead;
            Body = body;
            Details = details;
            Buttons = buttons;
            ToggleShowingDetailsCommand = new SyncCommand(ToggleShowingDetails);
        }

        public void ToggleShowingDetails()
        {
            ShowingDetails = !ShowingDetails;
        }
    }
}
