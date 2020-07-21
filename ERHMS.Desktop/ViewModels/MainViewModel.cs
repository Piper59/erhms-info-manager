using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.EpiInfo;
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

        public Command GoHomeCommand { get; }
        public Command OpenEpiInfoCommand { get; }
        public Command OpenFileExplorerCommand { get; }

        private MainViewModel()
        {
            GoHomeCommand = new SimpleSyncCommand(GoHome);
            OpenEpiInfoCommand = new SimpleSyncCommand(OpenEpiInfo);
            OpenFileExplorerCommand = new SimpleSyncCommand(OpenFileExplorer);
        }

        private void GoHome()
        {
            Content = new HomeViewModel();
        }

        private void OpenEpiInfo()
        {
            Module.Menu.Start();
        }

        private void OpenFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
