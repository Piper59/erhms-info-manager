using ERHMS.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        public static class Arguments
        {
            public static string Escape(string arg)
            {
                return string.Format("\"{0}\"", arg.Replace("\"", "\"\""));
            }

            public static string Format(IEnumerable<string> args)
            {
                return string.Join(" ", args.Select(arg => Escape(arg)));
            }

            private static string Format(KeyValuePair<string, string> arg)
            {
                return string.Format("/{0}:{1}", arg.Key, Escape(arg.Value));
            }

            public static string Format(IDictionary<string, string> args)
            {
                return string.Join(" ", args.Select(arg => Format(arg)));
            }
        }

        private static FileInfo GetExecutable(this Module @this)
        {
            string fileName = string.Format("{0}.exe", @this);
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        public static Process Execute(this Module @this, string args = "")
        {
            Log.Current.DebugFormat("Executing module: {0} {1}", @this, args);
            FileInfo executable = @this.GetExecutable();
            return ProcessExtensions.Start(executable, args);
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
