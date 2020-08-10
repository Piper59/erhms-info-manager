using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ObservableObject
    {
        public Icon Icon { get; }
        public string Lead { get; }
        public string Body { get; }
        public string Details { get; }
        public IReadOnlyCollection<DialogButton> Buttons { get; }

        private bool showingDetails;
        public bool ShowingDetails
        {
            get { return showingDetails; }
            set { SetProperty(ref showingDetails, value); }
        }

        public ICommand ToggleShowingDetailsCommand { get; }

        public DialogViewModel(DialogInfo info)
        {
            Icon = info.Icon;
            Lead = info.Lead;
            Body = info.Body;
            Details = info.Details;
            Buttons = info.Buttons;
            ToggleShowingDetailsCommand = new SyncCommand(ToggleShowingDetails, Command.Always, ErrorBehavior.Raise);
        }

        public void ToggleShowingDetails()
        {
            ShowingDetails = !ShowingDetails;
        }
    }
}
