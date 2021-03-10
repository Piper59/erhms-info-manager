using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace ERHMS.Launcher
{
    public class Program
    {
        private const string AppTitle = "ERHMS Info Manager";
        private const string BuildDirectoryName = AppTitle;
        private static readonly string ExecutableName = $"{AppTitle}.exe";

        [STAThread]
        private static void Main()
        {
            string executablePath = null;
            try
            {
                string buildContainerDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string workingDirectoryPath = Path.Combine(buildContainerDirectoryPath, BuildDirectoryName);
                executablePath = Path.Combine(workingDirectoryPath, ExecutableName);
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectoryPath,
                    FileName = executablePath
                });
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"{AppTitle} could not be launched");
                if (executablePath == null)
                {
                    message.AppendLine(".");
                }
                else
                {
                    message.AppendLine(" from the following location:");
                    message.AppendLine();
                    message.AppendLine(executablePath);
                }
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
