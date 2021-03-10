using ERHMS.Desktop.ViewModels;
using System.Windows.Input;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static ICommand ViewHomeCommand => MainViewModel.Instance.ViewHomeCommand;
        public static ICommand ViewCoreProjectCommand => MainViewModel.Instance.ViewCoreProjectCommand;
        public static ICommand ViewCoreViewCommand => MainViewModel.Instance.ViewCoreViewCommand;
    }
}
