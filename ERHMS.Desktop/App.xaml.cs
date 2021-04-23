using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Infrastructure.Services;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Utilities;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using System;
using System.Windows;
using ErrorEventArgs = ERHMS.Desktop.Commands.ErrorEventArgs;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        [STAThread]
        private static void Main()
        {
            Log.Configure(Log.Appenders.File);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Command.GlobalError += Command_GlobalError;
            Log.Instance.Debug("Starting up");
            try
            {
                UpgradeSettings();
                ConfigureEpiInfo();
                ConfigureServices();
                MenuDropAlignment.Value = false;
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                OnFatalException(ex);
            }
            finally
            {
                Log.Instance.Debug("Shutting down");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Instance.Fatal(e.ExceptionObject);
        }

        private static void Command_GlobalError(object sender, ErrorEventArgs e)
        {
            OnNonFatalException(e.Exception);
            e.Handled = true;
        }

        private static void OnFatalException(Exception exception)
        {
            Log.Instance.Fatal(exception);
            MessageBox.Show(
                string.Format(ResXResources.Body_FatalException, exception.Message),
                ResXResources.Title_App,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private static void OnNonFatalException(Exception exception)
        {
            Log.Instance.Error(exception);
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Error;
            dialog.Lead = ResXResources.Lead_NonFatalException;
            dialog.Body = exception.Message;
            dialog.Details = exception.ToString();
            dialog.Buttons = DialogButtonCollection.Close;
            dialog.Show();
        }

        private static bool UpgradeSettings()
        {
            if (!Settings.Default.UpgradeRequired)
            {
                return false;
            }
            Settings.Default.Upgrade();
            Settings.Default.UpgradeRequired = false;
            Settings.Default.Save();
            return true;
        }

        private static void ConfigureEpiInfo()
        {
            if (!ConfigurationExtensions.Exists())
            {
                Configuration configuration = ConfigurationExtensions.Create();
                configuration.Save();
            }
            ConfigurationExtensions.Load();
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
        }

        private static void ConfigureServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
            ServiceLocator.Install<IFileDialogService>(() => new FileDialogService());
            ServiceLocator.Install<IProgressService>(() => new ProgressService());
        }

        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                Log.Instance.Debug("Running in integration mode");
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
                MainViewModel.Instance.Content = new HomeViewModel();
                MainWindow = new MainView();
                MainWindow.Show();
            }
        }
    }
}
