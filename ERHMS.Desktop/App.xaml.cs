using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure.Services;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
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
            ConfigureLog();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Log.Default.Debug("Entering application");
            try
            {
                UpgradeSettings();
                ConfigureEpiInfo();
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Default.Fatal(ex);
                StringBuilder message = new StringBuilder();
                message.AppendLine(ResXResources.Body_CaughtFatalException);
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(
                    message.ToString(),
                    ResXResources.Title_App,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                Log.Default.Debug("Exiting application");
            }
        }

        private static void ConfigureLog()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch { }
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout layout = new PatternLayout(
                "%date | %property{user} | %property{process}(%thread) | %level | %message%newline");
            layout.ActivateOptions();
            FileAppender appender = new FileAppender
            {
                File = Log.DefaultFilePath,
                LockingModel = new FileAppender.InterProcessLock(),
                Layout = layout
            };
            appender.ActivateOptions();
            hierarchy.Root.AddAppender(appender);
            hierarchy.Configured = true;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Default.Fatal(e.ExceptionObject);
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

        public MainView MainView => (MainView)MainWindow;

        public App()
        {
            InitializeComponent();
            ConfigureServices();
            Command.GlobalError += Command_GlobalError;
        }

        private void ConfigureServices()
        {
            ServiceProvider.Install<IDialogService>(() => new DialogService(this));
            ServiceProvider.Install<IFileDialogService>(() => new FileDialogService(this));
            ServiceProvider.Install<INotificationService>(() => MainView);
            ServiceProvider.Install<IProgressService>(() => new ProgressService(this));
        }

        private void Command_GlobalError(object sender, ErrorEventArgs e)
        {
            Log.Default.Error(e.Exception);
            ServiceProvider.Resolve<IDialogService>().Show(
                DialogType.Error,
                ResXResources.Lead_CaughtNonFatalException,
                e.Exception.Message,
                e.Exception.ToString(),
                DialogButtonCollection.Close);
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainViewModel.Instance.Content = new HomeViewModel();
            MainWindow = new MainView
            {
                DataContext = MainViewModel.Instance
            };
            MainWindow.Show();
        }
    }
}
