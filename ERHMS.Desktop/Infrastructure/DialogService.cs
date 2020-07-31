using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Infrastructure
{
    internal class DialogService : IDialogService
    {
        public Application Application { get; }
        public Dispatcher Dispatcher => Application.Dispatcher;

        public DialogService(Application application)
        {
            Application = application;
        }

        public bool? Show(DialogInfo info)
        {
            if (Dispatcher.CheckAccess())
            {
                return ShowInternal(info);
            }
            else
            {
                return Dispatcher.Invoke(() => ShowInternal(info));
            }
        }

        private bool? ShowInternal(DialogInfo info)
        {
            Window owner = Application.MainWindow;
            Window window = new DialogView
            {
                DataContext = new DialogViewModel(info),
                Owner = owner
            };
            info.Sound?.Play();
            return window.ShowDialog();
        }
    }
}
