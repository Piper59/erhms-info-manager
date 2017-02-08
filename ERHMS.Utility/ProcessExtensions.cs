using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public static class ProcessExtensions
    {
        public static string EscapeArg(object arg)
        {
            return string.Format("\"{0}\"", (arg ?? "").ToString().Replace("\"", "\"\""));
        }

        public static string FormatArgs(IEnumerable<object> args)
        {
            return string.Join(" ", args.Select(arg => EscapeArg(arg)));
        }

        public static string FormatArgs(params object[] args)
        {
            return FormatArgs(args.AsEnumerable());
        }
    }
}
