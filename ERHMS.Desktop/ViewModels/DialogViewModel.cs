using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ERHMS.Desktop.ViewModels
{
    public class DialogViewModel : ObservableObject
    {
        public DialogSeverity Severity { get; }
        public ImageSource Image { get; }
        public string Lead { get; }
        public string Body { get; }
        public string Details { get; }
        public DialogButtonCollection Buttons { get; }
        public bool? Result { get; set; }

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
            DialogButtonCollection buttons)
        {
            Severity = severity;
            Icon icon = severity.ToIcon();
            if (icon != null)
            {
                Image = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            Lead = lead;
            Body = body;
            Details = details;
            Buttons = buttons;
            ToggleShowingDetailsCommand = new SyncCommand(ToggleShowingDetails);
        }

        public void ToggleShowingDetails()
        {
            ShowingDetails = !showingDetails;
        }
    }
}
