using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class DialogService : IDialogService
    {
        public Application Application { get; }

        public DialogService(Application application)
        {
            Application = application;
        }

        public bool? Show(
            DialogSeverity severity,
            string lead,
            string body,
            string details,
            DialogButtonCollection buttons)
        {
            Window window = new DialogView
            {
                Owner = Application.GetActiveOrMainWindow(),
                DataContext = new DialogViewModel(severity, lead, body, details, buttons)
            };
            severity.ToSystemSound()?.Play();
            return window.ShowDialog();
        }
    }
}
