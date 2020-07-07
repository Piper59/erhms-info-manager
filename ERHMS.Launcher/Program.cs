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
        private const string TargetFileName = "ERHMS.Desktop.exe";

        [STAThread]
        public static void Main(string[] args)
        {
            string targetFile = null;
            try
            {
                string buildContainerDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string buildDir = Path.Combine(buildContainerDir, BuildDirName);
                targetFile = Path.Combine(buildDir, TargetFileName);
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = buildDir,
                    FileName = targetFile
                });
            }
            catch
            {
                StringBuilder message = new StringBuilder();
                message.Append($"{AppTitle} could not be launched");
                if (targetFile == null)
                {
                    message.Append(".");
                }
                else
                {
                    message.AppendLine(" from the following location:");
                    message.AppendLine();
                    message.Append(targetFile);
                }
                MessageBox.Show(message.ToString(), AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
