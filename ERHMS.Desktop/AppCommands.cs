using ERHMS.Desktop.ViewModels;
using System.Windows.Input;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static ICommand ExitCommand => MainViewModel.Instance.ExitCommand;
        public static ICommand GoToHomeCommand => MainViewModel.Instance.GoToHomeCommand;
        public static ICommand GoToHelpCommand => MainViewModel.Instance.GoToHelpCommand;
        public static ICommand GoToCoreProjectCommand => MainViewModel.Instance.GoToCoreProjectCommand;
        public static ICommand GoToCoreViewCommand => MainViewModel.Instance.GoToCoreViewCommand;
    }
}
