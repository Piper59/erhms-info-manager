using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
        Updater,
        WebSurveyExporter
    }

    public static class ModuleExtensions
    {
        private static FileInfo GetExecutable(this Module @this)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = string.Format("{0}.exe", @this);
            return new FileInfo(Path.Combine(directoryName, fileName));
        }

        public static Process Execute(this Module @this, string args = "")
        {
            FileInfo executable = @this.GetExecutable();
            return Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = executable.FullName,
                    WorkingDirectory = executable.DirectoryName,
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
