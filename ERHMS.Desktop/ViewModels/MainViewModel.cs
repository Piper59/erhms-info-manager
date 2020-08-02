using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.EpiInfo;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Module = ERHMS.EpiInfo.Module;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Current { get; } = new MainViewModel();

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Default.Debug($"Displaying: {value}");
                SetProperty(ref content, value);
            }
        }

        public Command ExitCommand { get; }
        public Command GoToHomeCommand { get; }
        public Command StartEpiInfoCommand { get; }
        public Command StartFileExplorerCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SimpleSyncCommand(Exit);
            GoToHomeCommand = new SimpleSyncCommand(GoToHome);
            StartEpiInfoCommand = new SimpleSyncCommand(StartEpiInfo);
            StartFileExplorerCommand = new SimpleSyncCommand(StartFileExplorer);
        }

        public event EventHandler ExitRequested;

        private void OnExitRequested()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Exit()
        {
            OnExitRequested();
        }

        private void GoToHome()
        {
            Content = new HomeViewModel();
        }

        private void StartEpiInfo()
        {
            Module.Menu.Start();
        }

        private void StartFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
