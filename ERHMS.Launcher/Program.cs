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
        public static void Main(string[] args)
        {
            string executable = null;
            try
            {
                string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string workingDirectory = Path.Combine(currentDirectory, AppTitle);
                executable = Path.Combine(workingDirectory, $"{AppTitle}.exe");
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    FileName = executable
                });
            }
            catch (Exception ex)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"{AppTitle} could not be launched");
                if (executable == null)
                {
                    message.AppendLine(".");
                }
                else
                {
                    message.AppendLine(" from the following location:");
                    message.AppendLine();
                    message.AppendLine(executable);
                }
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
