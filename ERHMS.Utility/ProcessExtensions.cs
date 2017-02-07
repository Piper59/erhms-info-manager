using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ERHMS.Utility
{
    public static class ProcessExtensions
    {
        private static string EscapeArgument(string argument)
        {
            return string.Format("\"{0}\"", argument.Replace("\"", "\"\""));
        }

        private static string FormatArguments(IEnumerable<string> arguments)
        {
            return string.Join(" ", arguments.Select(argument => EscapeArgument(argument ?? "")));
        }

        public static Process Create(FileInfo executable, IEnumerable<string> arguments = null)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = executable.DirectoryName,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = executable.FullName,
                    Arguments = arguments == null ? "" : FormatArguments(arguments)
                }
            };
        }
    }
}
