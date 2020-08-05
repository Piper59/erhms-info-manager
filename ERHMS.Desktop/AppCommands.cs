using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static Command GoHomeCommand => MainViewModel.Current.GoHomeCommand;
        public static Command ViewWorkerProjectCommand => MainViewModel.Current.ViewWorkerProjectCommand;
        public static Command ViewIncidentProjectCommand => MainViewModel.Current.ViewIncidentProjectCommand;
    }
}
