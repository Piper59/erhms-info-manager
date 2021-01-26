using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using System;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private static readonly Lazy<MainViewModel> instance = new Lazy<MainViewModel>(GetInstance);
        public static MainViewModel Instance => instance.Value;

        private static MainViewModel GetInstance()
        {
            return new MainViewModel();
        }

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Instance.Debug($"Displaying content: {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand ExitCommand { get; }
        public ICommand ViewHomeCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SyncCommand(Exit);
            ViewHomeCommand = new SyncCommand(ViewHome);
        }

        public event EventHandler ExitRequested;
        private void OnExitRequested() => ExitRequested?.Invoke(this, EventArgs.Empty);

        public void Exit()
        {
            OnExitRequested();
        }

        public void ViewHome()
        {
            Content = new HomeViewModel();
        }
    }
}
