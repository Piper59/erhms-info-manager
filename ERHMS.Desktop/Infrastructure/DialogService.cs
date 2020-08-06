using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public class DialogService : IDialogService
    {
        private readonly Application application;
        private readonly DialogInfo info;
        private readonly DialogViewModel dataContext;

        public DialogService(Application application, DialogInfo info)
        {
            this.application = application;
            this.info = info;
            dataContext = new DialogViewModel(info);
        }

        public bool? Show()
        {
            if (application.Dispatcher.CheckAccess())
            {
                return ShowInternal();
            }
            else
            {
                return application.Dispatcher.Invoke(ShowInternal);
            }
        }

        private bool? ShowInternal()
        {
            Window owner = application.MainWindow;
            Window window = new DialogView
            {
                DataContext = dataContext,
                Owner = owner
            };
            info.Sound?.Play();
            return window.ShowDialog();
        }
    }
}
