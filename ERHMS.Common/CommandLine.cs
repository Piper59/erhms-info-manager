using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common
{
    public static class CommandLine
    {
        public static string Escape(string arg)
        {
            return arg.Replace("\"", "\"\"");
        }

        public static string Quote(string arg)
        {
            return $"\"{Escape(arg)}\"";
        }

        public static string Quote(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(Quote));
        }

        public static string Quote(params string[] args)
        {
            return Quote((IEnumerable<string>)args);
        }
    }
}
