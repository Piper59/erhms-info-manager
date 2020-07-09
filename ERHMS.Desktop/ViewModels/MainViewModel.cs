using ERHMS.Desktop.Commands;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using System.Diagnostics;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
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

        private ViewModelBase content;
        public ViewModelBase Content
        {
            get
            {
                return content;
            }
            set
            {
                string type = value?.GetType()?.Name ?? "NULL";
                Log.Default.Debug($"Displaying: {type}");
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
            Module.Menu.Start();
        }

        private void OpenFileExplorer()
        {
            Process.Start(ReflectionExtensions.GetEntryDirectory());
        }
    }
}
