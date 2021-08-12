using ERHMS.Desktop.Commands;
using System.Windows.Input;

namespace ERHMS.Desktop
{
    public static class AppCommands
    {
        private class NullImpl : IAppCommands
        {
            public ICommand GoToHomeCommand => Command.Null;
            public ICommand GoToHelpCommand => Command.Null;
            public ICommand GoToProjectCommand => Command.Null;
            public ICommand GoToCoreProjectCommand => Command.Null;
            public ICommand GoToViewCommand => Command.Null;
            public ICommand GoToCoreViewCommand => Command.Null;
            public ICommand CreateCoreProjectCommand => Command.Null;
            public ICommand OpenCoreProjectCommand => Command.Null;
            public ICommand OpenPathCommand => Command.Null;
        }

        public static IAppCommands Instance { get; set; } = new NullImpl();

        public static ICommand GoToHomeCommand => Instance.GoToHomeCommand;
        public static ICommand GoToHelpCommand => Instance.GoToHelpCommand;
        public static ICommand GoToCoreProjectCommand => Instance.GoToCoreProjectCommand;
        public static ICommand GoToProjectCommand => Instance.GoToProjectCommand;
        public static ICommand GoToCoreViewCommand => Instance.GoToCoreViewCommand;
        public static ICommand GoToViewCommand => Instance.GoToViewCommand;
        public static ICommand CreateCoreProjectCommand => Instance.CreateCoreProjectCommand;
        public static ICommand OpenCoreProjectCommand => Instance.OpenCoreProjectCommand;
        public static ICommand OpenPathCommand => Instance.OpenPathCommand;
    }
}
