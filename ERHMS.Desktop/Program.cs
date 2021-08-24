using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using System;
using System.Windows;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Log.Initialize(new FileAppender());
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Log.Instance.Debug("Starting up");
            try
            {
                Settings.Default.Initialize();
                Configuration.Initialize(ExecutionEnvironment.WindowsApplication);
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex);
                MessageBox.Show(
                    string.Format(Strings.Body_FatalError, ex.Message),
                    Strings.Title_App,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
    }
}
