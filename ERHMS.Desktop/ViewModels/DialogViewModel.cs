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
        public DialogSeverity Severity { get; }
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

        public DialogViewModel(
            DialogSeverity severity,
            string lead,
            string body,
            string details,
            IReadOnlyCollection<DialogButton> buttons)
        {
            Severity = severity;
            Icon = severity.ToIcon();
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
