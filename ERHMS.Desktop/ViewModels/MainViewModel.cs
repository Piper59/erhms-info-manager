using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.EpiInfo;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Default.Debug($"Displaying content: {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand ExitCommand { get; }
        public ICommand ViewHomeCommand { get; }
        public ICommand StartEpiInfoCommand { get; }
        public ICommand StartFileExplorerCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SyncCommand(Exit);
            ViewHomeCommand = new SyncCommand(ViewHome);
            StartEpiInfoCommand = new SyncCommand(StartEpiInfo);
            StartFileExplorerCommand = new SyncCommand(StartFileExplorer);
        }

        public event EventHandler ExitRequested;
        private void OnExitRequested(EventArgs e) => ExitRequested?.Invoke(this, e);
        private void OnExitRequested() => OnExitRequested(EventArgs.Empty);

        public void Exit()
        {
            OnExitRequested();
        }

        public void ViewHome()
        {
            Content = new HomeViewModel();
        }

        public void StartEpiInfo()
        {
            Module.Menu.Start();
        }

        public void StartFileExplorer()
        {
            string buildDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Process.Start(buildDirPath);
        }
    }
}
