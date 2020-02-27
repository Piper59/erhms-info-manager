using ERHMS.Desktop.Views;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ResXResources = ERHMS.Desktop.Properties.Resources;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static int errorCount;

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void HandleError(Exception ex)
        {
            if (Interlocked.Increment(ref errorCount) > 1)
            {
                return;
            }
            MessageBox.Show(GetErrorMessage(ex), $"{ResXResources.AppTitle} - Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static string GetErrorMessage(Exception ex)
        {
#if DEBUG
            return DEBUG_GetErrorMessage(ex);
#else
            return RELEASE_GetErrorMessage(ex);
#endif
        }

        private static string DEBUG_GetErrorMessage(Exception ex)
        {
            return ex.ToString();
        }

        private static string RELEASE_GetErrorMessage(Exception ex)
        {
            return string.Format(ResXResources.AppError, ex.Message);
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
            MainView mainView = new MainView();
            mainView.Show();
        }
    }
}
