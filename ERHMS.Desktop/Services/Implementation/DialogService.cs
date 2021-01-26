using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Services.Implementation
{
    public class DialogService : IDialogService
    {
        private readonly Application application;

        public DialogService(Application application)
        {
            this.application = application;
        }

        public bool? Show(Dialog dialog)
        {
            Window owner = application.GetActiveOrMainWindow();
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
