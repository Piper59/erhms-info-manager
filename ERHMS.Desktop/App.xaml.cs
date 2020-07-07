using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ResXResources = ERHMS.Desktop.Properties.Resources;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static int errorCount = 0;

        public static string BuildDir { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Log.Default.Debug("Starting up");
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
            Log.Default.Error(ex.Message, ex);
            if (Interlocked.Increment(ref errorCount) > 1)
            {
                return;
            }
            string message = string.Format(ResXResources.AppError, Log.Default.Logger.Repository.GetFile());
            MessageBox.Show(message, ResXResources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
            Window window = new MainView(new MainViewModel());
            window.Show();
        }
    }
}
