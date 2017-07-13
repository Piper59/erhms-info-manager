using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo
{
    public class Pgm
    {
        public const string FileExtension = ".pgm7";
        private static readonly ICollection<Func<Pgm, object>> Identifiers = new Func<Pgm, object>[]
        {
            @this => @this.PgmId,
            @this => @this.Name,
            @this => @this.Content,
            @this => @this.Comment,
            @this => @this.Author
        };

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
            return ObjectExtensions.GetHashCode(this, Identifiers);
        }

        public override bool Equals(object obj)
        {
            return ObjectExtensions.Equals(this, obj, Identifiers);
        }
    }
}
