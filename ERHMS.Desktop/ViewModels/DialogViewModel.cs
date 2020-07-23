using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using System.Drawing;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ObservableObject
    {
        private Icon icon;
        public Icon Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private string lead;
        public string Lead
        {
            get { return lead; }
            set { SetProperty(ref lead, value); }
        }

        private string body;
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }

        private string details;
        public string Details
        {
            get { return details; }
            set { SetProperty(ref details, value); }
        }

        private DialogButtonCollection buttons;
        public DialogButtonCollection Buttons
        {
            get { return buttons; }
            set { SetProperty(ref buttons, value); }
        }

        private bool showingDetails;
        public bool ShowingDetails
        {
            get { return showingDetails; }
            set { SetProperty(ref showingDetails, value); }
        }

        public Command ToggleShowingDetailsCommand { get; }

        public DialogViewModel(DialogInfo info)
        {
            icon = info.Icon;
            lead = info.Lead;
            body = info.Body;
            details = info.Details;
            buttons = info.Buttons;
            ToggleShowingDetailsCommand = new SimpleSyncCommand(ToggleShowingDetails);
        }

        private void ToggleShowingDetails()
        {
            ShowingDetails = !ShowingDetails;
        }
    }
}
