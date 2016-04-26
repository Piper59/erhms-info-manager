using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ERHMS
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
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
    }
}
