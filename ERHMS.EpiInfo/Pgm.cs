using System;
using System.Data;

namespace ERHMS.EpiInfo
{
    public class Pgm
    {
        public const string FileExtension = ".pgm7";

        private static string GetContent(string location, string source)
        {
            return string.Format("READ {{{0}}}:[{1}]{2}", location, source, Environment.NewLine);
        }

        public static string GetContentForView(string projectPath, string viewName)
        {
            return GetContent(projectPath, viewName);
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

        public Pgm()
        {
            Comment = "";
            Author = "";
        }

        internal Pgm(DataRow row)
        {
            PgmId = row.Field<int>("ProgramId");
            Name = row.Field<string>("Name");
            Content = row.Field<string>("Content");
            Comment = row.Field<string>("Comment");
            Author = row.Field<string>("Author");
        }

        public override int GetHashCode()
        {
            return PgmId;
        }

        public override bool Equals(object obj)
        {
            Pgm pgm = obj as Pgm;
            return pgm != null && pgm.PgmId != 0 && pgm.PgmId == PgmId;
        }
    }
}
