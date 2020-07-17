using ERHMS.Desktop.Infrastructure;
using ERHMS.EpiInfo;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModel
    {
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

        private ViewModel content;
        public ViewModel Content
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

        public ICommand GoHomeCommand { get; }
        public ICommand OpenEpiInfoCommand { get; }
        public ICommand OpenFileExplorerCommand { get; }

        private MainViewModel()
        {
            GoHomeCommand = new SimpleCommand(GoHome);
            OpenEpiInfoCommand = new SimpleCommand(OpenEpiInfo);
            OpenFileExplorerCommand = new SimpleCommand(OpenFileExplorer);
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
