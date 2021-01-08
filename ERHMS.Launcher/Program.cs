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

        [STAThread]
        private static void Main()
        {
            string programPath = null;
            try
            {
                string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string workingDirectory = Path.Combine(currentDirectory, AppTitle);
                programPath = Path.Combine(workingDirectory, $"{AppTitle}.exe");
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    FileName = programPath
                });
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"{AppTitle} could not be launched");
                if (programPath == null)
                {
                    message.AppendLine(".");
                }
                else
                {
                    message.AppendLine(" from the following location:");
                    message.AppendLine();
                    message.AppendLine(programPath);
                }
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
