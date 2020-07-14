using Epi;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ResX = ERHMS.Desktop.Properties.Resources;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static int errorCount;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Log.Default.Debug("Starting up");
                Configure();
                App app = new App();
                app.Run();
                Log.Default.Debug("Shutting down");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void HandleError(Exception ex)
        {
            Log.Default.Error(ex);
            if (Interlocked.Increment(ref errorCount) > 1)
            {
                return;
            }
            string file = Log.Default.Logger.Repository.GetFile(Log.MainAppenderName);
            string message =
                file == null
                ? ResX.AppErrorWithoutLog
                : string.Format(ResX.AppErrorWithLog, file);
            MessageBox.Show(message, ResX.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void Configure()
        {
            Log.Default.Debug("Configuring");
            if (!ConfigurationExtensions.Exists())
            {
                Configuration configuration = ConfigurationExtensions.Create();
                Settings.Default.Apply(configuration);
                configuration.Save();
            }
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
