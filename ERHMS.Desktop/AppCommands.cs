using ERHMS.Desktop.ViewModels;
using System.Windows.Input;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static ICommand GoHomeCommand => MainViewModel.Current.GoHomeCommand;
        public static ICommand ViewWorkerProjectCommand => MainViewModel.Current.ViewWorkerProjectCommand;
        public static ICommand ViewIncidentProjectCommand => MainViewModel.Current.ViewIncidentProjectCommand;
        public static ICommand ViewCoreViewCommand => MainViewModel.Current.ViewCoreViewCommand;
    }
}
