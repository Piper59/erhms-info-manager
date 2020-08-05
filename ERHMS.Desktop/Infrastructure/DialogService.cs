using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Infrastructure
{
    public class DialogService : IDialogService
    {
        public Application Application { get; }
        public Dispatcher Dispatcher => Application.Dispatcher;
        public DialogInfo Info { get; }
        public DialogViewModel DataContext { get; }

        public DialogService(Application application, DialogInfo info)
        {
            Application = application;
            Info = info;
            DataContext = new DialogViewModel(info);
        }

        public bool? Show()
        {
            if (Dispatcher.CheckAccess())
            {
                return ShowInternal();
            }
            else
            {
                return Dispatcher.Invoke(ShowInternal);
            }
        }

        private bool? ShowInternal()
        {
            Window owner = Application.MainWindow;
            Window window = new DialogView
            {
                DataContext = DataContext,
                Owner = owner
            };
            Info.Sound?.Play();
            return window.ShowDialog();
        }
    }
}
