using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
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
        public DialogButtonCollection Buttons { get; }
        public bool? Result { get; set; }

        private bool detailed;
        public bool Detailed
        {
            get { return detailed; }
            set { SetProperty(ref detailed, value); }
        }

        public ICommand ToggleDetailedCommand { get; }

        public DialogViewModel(
            DialogSeverity severity,
            string lead,
            string body,
            string details,
            DialogButtonCollection buttons)
        {
            Severity = severity;
            Icon = severity.ToIcon();
            Lead = lead;
            Body = body;
            Details = details;
            Buttons = buttons;
            ToggleDetailedCommand = new SyncCommand(ToggleDetailed);
        }

        public void ToggleDetailed()
        {
            Detailed = !Detailed;
        }
    }
}
