﻿using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Events;
using ERHMS.Desktop.Infrastructure.Services;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Utilities;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;
using ErrorEventArgs = ERHMS.Desktop.Commands.ErrorEventArgs;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        public MainViewModel Main { get; } = new MainViewModel();

        public App()
        {
            InitializeComponent();
            InitializeServices();
            InitializeCommands();
            InitializeEventHandlers();
        }

        private void InitializeServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
            ServiceLocator.Install<IDirectoryDialogService>(() => new DirectoryDialogService());
            ServiceLocator.Install<IFileDialogService>(() => new FileDialogService());
            ServiceLocator.Install<IProgressService>(() => new ProgressService());
            ServiceLocator.Install<IWindowService>(() => new WindowService());
        }

        private void InitializeCommands()
        {
            AppCommands.Instance = Main;
            Command.GlobalError += Command_GlobalError;
        }

        private void Command_GlobalError(object sender, ErrorEventArgs e)
        {
            Log.Instance.Error(e.Exception);
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Error;
            dialog.Lead = Strings.Lead_NonFatalError;
            dialog.Body = e.Exception.Message;
            dialog.Details = e.Exception.ToString();
            dialog.Buttons = DialogButtonCollection.Close;
            dialog.Show();
            e.Handled = true;
        }

        private void InitializeEventHandlers()
        {
            OpenUriOnRequestNavigate.Register();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                Log.Instance.Debug("Running in utility mode");
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                try
                {
                    await Utility.ExecuteAsync(e.Args);
                }
                finally
                {
                    Shutdown();
                }
            }
            else
            {
                Log.Instance.Debug("Running in standard mode");
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                MainWindow = new MainView
                {
                    DataContext = Main
                };
                MainWindow.Show();
            }
        }
    }
}
