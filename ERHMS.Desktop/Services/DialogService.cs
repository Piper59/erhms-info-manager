using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Services
{
    internal class DialogService : IDialogService
    {
        private readonly Application application;

        public DialogService(Application application)
        {
            this.application = application;
        }

        public bool? Show(DialogInfo info)
        {
            if (application.Dispatcher.CheckAccess())
            {
                return ShowInternal(info);
            }
            else
            {
                return application.Dispatcher.Invoke(() => ShowInternal(info));
            }
        }

        private bool? ShowInternal(DialogInfo info)
        {
            Window owner = application.GetActiveWindow();
            Window window = new DialogView
            {
                Owner = owner,
                DataContext = new DialogViewModel(info)
            };
            info.Sound?.Play();
            return window.ShowDialog();
        }
    }
}
