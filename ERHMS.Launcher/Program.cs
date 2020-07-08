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
        private const string BuildDirectoryName = AppTitle;
        private const string ExecutableName = "ERHMS.Desktop.exe";

        [STAThread]
        public static void Main(string[] args)
        {
            string executable = null;
            try
            {
                string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string workingDirectory = Path.Combine(currentDirectory, BuildDirectoryName);
                executable = Path.Combine(workingDirectory, ExecutableName);
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    FileName = executable
                });
            }
            catch
            {
                StringBuilder message = new StringBuilder();
                message.Append($"{AppTitle} could not be launched");
                if (executable == null)
                {
                    message.Append(".");
                }
                else
                {
                    message.AppendLine(" from the following location:");
                    message.AppendLine();
                    message.Append(executable);
                }
                MessageBox.Show(message.ToString(), AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
