using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using System;
using System.Windows;

namespace ERHMS.Desktop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Log.Initialize(new FileAppender());
                Log.Instance.Debug("Starting up");
                Configuration.Initialize();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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
