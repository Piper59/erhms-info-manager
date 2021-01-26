using ERHMS.Desktop.ViewModels;
using System.Windows.Input;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        public static ICommand ExitCommand => MainViewModel.Instance.ExitCommand;
        public static ICommand ViewHomeCommand => MainViewModel.Instance.ViewHomeCommand;
    }
}
