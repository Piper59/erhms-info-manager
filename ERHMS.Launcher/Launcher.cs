using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ERHMS.Launcher
{
    public static class Launcher
    {
        internal static string GetEntryDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static void Execute(string path, string[] args)
        {
            try
            {
                string executable = Path.Combine(path, "ERHMS.Presentation.exe");
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = path,
                    FileName = executable
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERHMS Info Manager\u2122", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Execute(string[] args)
        {
            Execute(GetEntryDirectoryPath(), args);
        }
    }
}
