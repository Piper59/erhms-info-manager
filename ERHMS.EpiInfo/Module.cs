using ERHMS.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ERHMS.EpiInfo
{
    public enum Module
    {
        Analysis,
        AnalysisDashboard,
        Config,
        DataPackager,
        DataUnpackager,
        Enter,
        EpiInfo,
        MakeView,
        Mapping,
        Menu,
        WebSurveyExporter
    }

    public static class ModuleExtensions
    {
        private static FileInfo GetExecutable(this Module @this)
        {
            string fileName = string.Format("{0}.exe", @this);
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        public static Process Execute(this Module @this, string args = "")
        {
            FileInfo executable = @this.GetExecutable();
            return Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = executable.DirectoryName,
                FileName = executable.FullName,
                Arguments = args
            });
        }

        public static Process Execute(this Module @this, IEnumerable<string> args)
        {
            return @this.Execute(Arguments.Format(args));
        }

        public static Process Execute(this Module @this, IDictionary<string, string> args)
        {
            return @this.Execute(Arguments.Format(args));
        }
    }
}
