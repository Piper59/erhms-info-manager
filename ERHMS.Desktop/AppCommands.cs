using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static Command GoToHomeCommand => MainViewModel.Current.GoToHomeCommand;
        public static Command OpenWorkerProjectCommand => MainViewModel.Current.OpenWorkerProjectCommand;
        public static Command OpenIncidentProjectCommand => MainViewModel.Current.OpenIncidentProjectCommand;
        public static Command OpenViewCommand => MainViewModel.Current.OpenViewCommand;
    }
}
