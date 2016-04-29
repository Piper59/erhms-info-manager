using Epi;
using System;

namespace ERHMS.EpiInfo
{
    public class Pgm
    {
        public const string FileExtension = ".pgm7";

        private static string GetContent(string location, string source)
        {
            return string.Format("READ {{{0}}}:[{1}]{2}", location, source, Environment.NewLine);
        }

        public static string GetContentForView(View view)
        {
            return GetContent(view.Project.FilePath, view.Name);
        }

        public static string GetContentForTable(string connectionString, string tableName)
        {
            return GetContent(connectionString, tableName);
        }

        public int PgmId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
    }
}
