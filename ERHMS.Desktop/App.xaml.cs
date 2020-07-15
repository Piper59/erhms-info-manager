using Epi;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ResX = ERHMS.Desktop.Properties.Resources;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static ILog Log { get; set; }

        private static int errorCount;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                ConfigureLog();
                Log.Debug("Starting up");
                ConfigureEpiInfo();
                App app = new App();
                app.Run();
                Log.Debug("Shutting down");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void ConfigureLog()
        {
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException) { }
            XmlConfigurator.Configure();
            Log = LogManager.GetLogger(nameof(ERHMS));
        }

        private static void HandleError(Exception ex)
        {
            Log.Fatal(ex);
            if (Interlocked.Increment(ref errorCount) > 1)
            {
                return;
            }
            string file = Log.Logger.Repository.GetAppenders()
                .OfType<FileAppender>()
                .FirstOrDefault()
                ?.File;
            string message =
                file == null
                ? ResX.AppErrorWithoutLog
                : string.Format(ResX.AppErrorWithLog, file);
            MessageBox.Show(message, ResX.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void ConfigureEpiInfo()
        {
            Log.Debug("Configuring Epi Info");
            if (!ConfigurationExtensions.Exists())
            {
                Log.Debug($"Creating configuration file: {ConfigurationExtensions.FilePath}");
                Configuration configuration = ConfigurationExtensions.Create();
                Settings.Default.Apply(configuration);
                configuration.Save();
            }
            Log.Debug($"Loading configuration file: {ConfigurationExtensions.FilePath}");
            ConfigurationExtensions.Load();
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
        }

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            InitializeComponent();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleError(e.Exception);
            e.Handled = true;
            Shutdown(1);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainViewModel.Current.Content = new HomeViewModel();
            Window window = new MainView
            {
                DataContext = MainViewModel.Current
            };
            window.Show();
        }
    }
}
