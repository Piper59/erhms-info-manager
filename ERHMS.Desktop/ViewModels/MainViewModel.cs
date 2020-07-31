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
        public Command ViewHomeCommand { get; }
        public Command ViewEpiInfoCommand { get; }
        public Command ViewFileExplorerCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SimpleSyncCommand(Exit);
            ViewHomeCommand = new SimpleSyncCommand(ViewHome);
            ViewEpiInfoCommand = new SimpleSyncCommand(ViewEpiInfo);
            ViewFileExplorerCommand = new SimpleSyncCommand(ViewFileExplorer);
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

        private void ViewHome()
        {
            Content = new HomeViewModel();
        }

        private void ViewEpiInfo()
        {
            Module.Menu.Start();
        }

        private void ViewFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
