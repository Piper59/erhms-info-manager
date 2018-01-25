using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ERHMS.Launcher
{
    public static class Launcher
    {
        public static void Execute(string[] args)
        {
            try
            {
                DirectoryInfo directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                string executable = Path.Combine(directory.FullName, "ERHMS.Presentation.exe");
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = directory.FullName,
                    FileName = executable
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERHMS Info Manager\u2122", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
