using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
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
        public new MainView MainWindow
        {
            get { return (MainView)base.MainWindow; }
            set { base.MainWindow = value; }
        }

        public App()
        {
            InitializeComponent();
            InitializeServices();
            MenuDropAlignment.Value = false;
            Command.GlobalError += Command_GlobalError;
        }

        private void InitializeServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
            ServiceLocator.Install<IFileDialogService>(() => new FileDialogService());
            ServiceLocator.Install<IProgressService>(() => new ProgressService());
        }

        private void Command_GlobalError(object sender, ErrorEventArgs e)
        {
            Log.Instance.Error(e.Exception);
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Warning;
            dialog.Lead = ResXResources.Lead_NonFatalError;
            dialog.Body = e.Exception.Message;
            dialog.Details = e.Exception.ToString();
            dialog.Buttons = DialogButtonCollection.Close;
            dialog.Show();
            e.Handled = true;
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
                MainViewModel.Instance.GoToHome();
                MainWindow = new MainView
                {
                    DataContext = MainViewModel.Instance
                };
                MainWindow.Show();
            }
        }
    }
}
