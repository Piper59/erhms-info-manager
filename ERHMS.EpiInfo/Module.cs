using ERHMS.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        private static string ToFileName(this Module @this)
        {
            switch (@this)
            {
                case Module.Analysis:
                case Module.AnalysisDashboard:
                case Module.Enter:
                case Module.MakeView:
                    return $"{@this}.exe";
                case Module.Menu:
                    return "EpiInfo.exe";
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        private static string Quote(string arg)
        {
            return string.Format("\"{0}\"", arg.Replace("\"", "\"\""));
        }

        public static Process Start(this Module @this, params string[] args)
        {
            Log.Instance.Debug($"Starting module: {@this}");
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @this.ToFileName()),
                Arguments = string.Join(" ", args.Select(Quote))
            });
        }
    }
}
