using ERHMS.Common;
using System;
using System.Diagnostics;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ResetSettings : Utility
    {
        public override void Run()
        {
            string workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = workingDirectoryPath,
                FileName = Path.Combine(workingDirectoryPath, "ERHMS Info Manager.exe"),
                Arguments = CommandLine.Quote("ResetSettings", bool.FalseString)
            };
            Process.Start(startInfo)?.Dispose();
        }
    }
}
