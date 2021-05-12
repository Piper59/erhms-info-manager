using ERHMS.Launcher.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ERHMS.Launcher
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                string appTitle = "ERHMS Info Manager";
                string workingDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appTitle);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectoryPath,
                    FileName = Path.Combine(workingDirectoryPath, $"{appTitle}.exe")
                };
                using (Process.Start(startInfo)) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Body_Exception, ex.Message),
                    Resources.Title_App,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
