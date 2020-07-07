using ERHMS.Desktop.Commands;
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
                App.Log.Debug($"Displaying {value}");
                Set(ref content, value);
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
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = App.BuildDir,
                FileName = Path.Combine(App.BuildDir, "EpiInfo.exe")
            });
        }

        private void OpenFileExplorer()
        {
            Process.Start(App.BuildDir);
        }
    }
}
