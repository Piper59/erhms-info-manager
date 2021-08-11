using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        private bool minimized;
        public bool Minimized
        {
            get { return minimized; }
            set { SetProperty(ref minimized, value); }
        }

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            set { SetProperty(ref closed, value); }
        }

        public ICommand ToggleCommand { get; }
        public ICommand CloseCommand { get; }

        public StartViewModel()
        {
            Closed = Settings.Default.HasWorkerProjectPath && Settings.Default.HasIncidentProjectPath;
            ToggleCommand = new SyncCommand(Toggle);
            CloseCommand = new SyncCommand(Close);
        }

        public void Toggle()
        {
            Minimized = !Minimized;
        }

        public void Close()
        {
            Closed = true;
        }
    }
}
