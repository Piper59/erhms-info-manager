using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ERHMS.Launcher
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string workingDirectory = Path.Combine(path, "ERHMS Info Manager");
                string fileName = Path.Combine(workingDirectory, "ERHMS.Desktop.exe");
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    FileName = fileName
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERHMS Info Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
