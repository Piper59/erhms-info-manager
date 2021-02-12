using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace ERHMS.Launcher
{
    public class Program
    {
        private const string AppTitle = "ERHMS Info Manager";
        private const string BuildDirName = AppTitle;
        private const string ExecutableName = AppTitle + ".exe";

        [STAThread]
        private static void Main()
        {
            string executablePath = null;
            try
            {
                string buildContainerDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string workingDirPath = Path.Combine(buildContainerDirPath, BuildDirName);
                executablePath = Path.Combine(workingDirPath, ExecutableName);
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirPath,
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
