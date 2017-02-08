using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public static class ProcessExtensions
    {
        public static string EscapeArg(string arg)
        {
            return string.Format("\"{0}\"", (arg ?? "").Replace("\"", "\"\""));
        }

        public static string FormatArgs(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(arg => EscapeArg(arg)));
        }

        public static string FormatArgs(params string[] args)
        {
            return FormatArgs(args.AsEnumerable());
        }
    }
}
