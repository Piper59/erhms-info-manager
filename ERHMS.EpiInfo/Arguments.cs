using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo
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
}
