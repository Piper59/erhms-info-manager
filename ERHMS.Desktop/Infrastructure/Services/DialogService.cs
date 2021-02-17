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

        public bool? Show(Dialog dialog)
        {
            Window owner = Application.GetActiveOrMainWindow();
            Window window = new DialogView
            {
                Owner = owner,
                DataContext = new DialogViewModel(dialog)
            };
            dialog.Sound?.Play();
            return window.ShowDialog();
        }
    }
}
