using ERHMS.Desktop.Commands;
using ERHMS.Utility;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object content = new HomeViewModel();
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Default.Debug($"Displaying {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand HomeCommand { get; }
        public ICommand OpenEpiInfoCommand { get; }
        public ICommand OpenFileExplorerCommand { get; }

        public MainViewModel()
        {
            HomeCommand = new Command(Home);
            OpenEpiInfoCommand = new Command(OpenEpiInfo);
            OpenFileExplorerCommand = new Command(OpenFileExplorer);
        }

        private void Home()
        {
            Content = new HomeViewModel();
        }

        private void OpenEpiInfo()
        {
            string entryDir = ReflectionExtensions.GetEntryDir();
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = entryDir,
                FileName = Path.Combine(entryDir, "EpiInfo.exe")
            });
        }

        private void OpenFileExplorer()
        {
            Process.Start(ReflectionExtensions.GetEntryDir());
        }
    }
}
