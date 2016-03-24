using System.Diagnostics;
using System.IO;

namespace ERHMS.Utility
{
    public static class ProcessExtensions
    {
        public static Process Start(FileInfo executable, string args = "")
        {
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = executable.DirectoryName,
                FileName = executable.FullName,
                Arguments = args
            });
        }
    }
}
