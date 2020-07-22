using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure
{
    public class DialogService : IDialogService
    {
        public bool? Show(DialogInfo info)
        {
            if (Application.Current.CheckAccess())
            {
                return ShowInternal(info);
            }
            return Application.Current.Dispatcher.Invoke(() => ShowInternal(info));
        }

        private bool? ShowInternal(DialogInfo info)
        {
            Window owner = Application.Current.MainWindow;
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
