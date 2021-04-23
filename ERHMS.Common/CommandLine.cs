using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common
{
    public static class CommandLine
    {
        public static string Quote(string arg)
        {
            return string.Format("\"{0}\"", arg.Replace("\"", "\"\""));
        }

        public static string GetArguments(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(Quote));
        }
    }
}
