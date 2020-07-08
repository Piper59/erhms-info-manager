using ERHMS.Utility;
using System.Diagnostics;
using System.IO;

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
        public static string GetFileName(this Module @this)
        {
            switch (@this)
            {
                case Module.Menu:
                    return "EpiInfo.exe";
                default:
                    return $"{@this}.exe";
            }
        }

        public static Process Start(this Module @this, string arguments = "")
        {
            string entryDirectory = ReflectionExtensions.GetEntryDirectory();
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = entryDirectory,
                FileName = Path.Combine(entryDirectory, @this.GetFileName()),
                Arguments = arguments
            });
        }
    }
}
