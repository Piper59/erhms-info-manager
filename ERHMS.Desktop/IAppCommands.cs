﻿using System.Windows.Input;

namespace ERHMS.Desktop
{
    public interface IAppCommands
    {
        ICommand GoToHomeCommand { get; }
        ICommand GoToHelpCommand { get; }
        ICommand GoToProjectCommand { get; }
        ICommand GoToCoreProjectCommand { get; }
        ICommand GoToViewCommand { get; }
        ICommand GoToCoreViewCommand { get; }
        ICommand CreateCoreProjectCommand { get; }
        ICommand OpenCoreProjectCommand { get; }
        ICommand SetUpCoreProjectCommand { get; }
        ICommand OpenUriCommand { get; }
    }
}
