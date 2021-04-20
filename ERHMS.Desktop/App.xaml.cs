using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Infrastructure.Services;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using System;
using System.Linq;
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
            Log.Configure(Log.Appenders.File);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Command.GlobalError += Command_GlobalError;
            Log.Instance.Debug("Entering application");
            try
            {
                UpgradeSettings();
                ConfigureServices();
                ConfigureEpiInfo();
                MenuDropAlignment.Value = false;
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                OnFatalException(ex);
            }
            Log.Instance.Debug("Exiting application");
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

        internal static void OnFatalException(Exception exception)
        {
            Log.Instance.Fatal(exception);
            StringBuilder message = new StringBuilder();
            message.AppendLine(ResXResources.Body_FatalException);
            message.AppendLine();
            message.Append(exception.Message);
            MessageBox.Show(
                message.ToString(),
                ResXResources.Title_App,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        internal static void OnNonFatalException(Exception exception)
        {
            Log.Instance.Error(exception);
            IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
            dialog.Severity = DialogSeverity.Error;
            dialog.Lead = ResXResources.Lead_NonFatalException;
            if (exception is AggregateException aggregateException)
            {
                dialog.Body = aggregateException.Flatten().InnerExceptions.First().Message;
            }
            else
            {
                dialog.Body = exception.Message;
            }
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

        private static void ConfigureServices()
        {
            ServiceLocator.Install<IDialogService>(() => new DialogService());
            ServiceLocator.Install<IFileDialogService>(() => new FileDialogService());
            ServiceLocator.Install<IProgressService>(() => new ProgressService());
            ServiceLocator.Install<IWindowingService>(() => new WpfWindowingService());
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

        public new MainView MainWindow
        {
            get { return (MainView)base.MainWindow; }
            private set { base.MainWindow = value; }
        }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainViewModel.Instance.Content = new HomeViewModel();
            MainWindow = new MainView();
            MainWindow.Show();
        }
    }
}
