using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
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
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.Providers.Jet4,
                DataSource = Path.GetDirectoryName(path)
            };
            builder["Extended Properties"] = "text;HDR=Yes;FMT=Delimited";
            return builder.ConnectionString;
        }

        private static string GetCsvFileName(string path)
        {
            return Path.GetFileNameWithoutExtension(path) + "#csv";
        }

        public static string Assign(string source, string target)
        {
            return string.Format("ASSIGN {0} = {1}", target, Escape(source));
        }

        public static string Define(string variableName)
        {
            return string.Format("DEFINE {0}", variableName);
        }

        public static string MergeCsv(string path, string id)
        {
            return string.Format(
                "MERGE {{{0}}}:{1} GlobalRecordId :: {2}",
                GetCsvConnectionString(path),
                GetCsvFileName(path),
                id);
        }

        public static string Read(string projectPath, string viewName)
        {
            return string.Format("READ {{{0}}}:{1}", projectPath, viewName);
        }

        public static string WriteCsv(string path, IEnumerable<string> variableNames)
        {
            return string.Format(
                "WRITE REPLACE \"TEXT\" {{{0}}} : [{1}] {2}",
                GetCsvConnectionString(path),
                GetCsvFileName(path),
                string.Join(" ", variableNames.Select(variableName => Escape(variableName))));
        }
    }
}
