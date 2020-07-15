using ERHMS.Desktop.Commands;
using ERHMS.EpiInfo;
using log4net;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        protected static ILog Log { get; } = LogManager.GetLogger(nameof(ERHMS));

        private static MainViewModel current;
        public static MainViewModel Current
        {
            get
            {
                if (current == null)
                {
                    current = new MainViewModel();
                }
                return current;
            }
        }

        private ViewModelBase content;
        public ViewModelBase Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Debug($"Displaying: {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand GoHomeCommand { get; }
        public ICommand OpenEpiInfoCommand { get; }
        public ICommand OpenFileExplorerCommand { get; }

        private MainViewModel()
        {
            GoHomeCommand = new Command(GoHome);
            OpenEpiInfoCommand = new Command(OpenEpiInfo);
            OpenFileExplorerCommand = new Command(OpenFileExplorer);
        }

        private void GoHome()
        {
            Content = new HomeViewModel();
        }

        private void OpenEpiInfo()
        {
            EpiInfo.Module.Menu.Start();
        }

        private void OpenFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
