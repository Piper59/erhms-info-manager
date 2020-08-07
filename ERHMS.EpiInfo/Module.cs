using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERHMS.EpiInfo
{
    public enum Module
    {
        Analysis,
        AnalysisDashboard,
        Enter,
        MakeView,
        Menu
    }

    public static class ModuleExtensions
    {
        public static string ToFileName(this Module @this)
        {
            switch (@this)
            {
                case Module.Menu:
                    return "EpiInfo.exe";
                default:
                    return $"{@this}.exe";
            }
        }

        public static Process Start(this Module @this, params string[] arguments)
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = entryDirectory,
                FileName = Path.Combine(entryDirectory, @this.ToFileName()),
                Arguments = string.Join(" ", arguments.Select(argument => $"\"{argument.Replace("\"", "\"\"")}\""))
            });
        }
    }
}
