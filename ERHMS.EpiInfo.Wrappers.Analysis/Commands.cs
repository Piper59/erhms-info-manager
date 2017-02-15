using Epi;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace ERHMS.EpiInfo.Wrappers
{
    internal static class Commands
    {
        private static string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier);
        }

        private static string GetCsvConnectionString(string path)
        {
            DbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.Jet.OLEDB.4.0",
                DataSource = Path.GetDirectoryName(path)
            };
            builder["Extended Properties"] = "text;HDR=Yes;FMT=Delimited";
            return builder.ConnectionString;
        }

        public static string Assign(string source, string target)
        {
            return string.Format("ASSIGN {0} = {1}", target, Escape(source));
        }

        public static string Define(string name)
        {
            return string.Format("DEFINE {0}", name);
        }

        public static string MergeCsv(string path, string key)
        {
            string format = "MERGE {{{0}}}:{1} {2} :: {3}";
            return string.Format(format, GetCsvConnectionString(path), Path.GetFileName(path), ColumnNames.GLOBAL_RECORD_ID, key);
        }

        public static string Read(string projectPath, string viewName)
        {
            return string.Format("READ {{{0}}}:{1}", projectPath, viewName);
        }

        public static string Type(string value)
        {
            return string.Format("TYPEOUT \"{0}\"", value.Replace("\"", ""));
        }

        public static string WriteCsv(string path, IEnumerable<string> names)
        {
            string format = "WRITE REPLACE \"TEXT\" {{{0}}} : [{1}] {2}";
            IEnumerable<string> escapedNames = names.Select(name => Escape(name));
            return string.Format(format, GetCsvConnectionString(path), Path.GetFileName(path), escapedNames);
        }
    }
}
