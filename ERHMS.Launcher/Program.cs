using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
                string workingDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERHMS Info Manager");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectoryPath,
                    FileName = Path.Combine(workingDirectoryPath, "ERHMS Info Manager.exe")
                };
                Process.Start(startInfo)?.Dispose();
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("ERHMS Info Manager has encountered an error and must shut down.");
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), "ERHMS Info Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
