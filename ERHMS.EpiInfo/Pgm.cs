using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class Pgm
    {
        public const string FileExtension = ".pgm7";

        public static IEnumerable<Pgm> GetByProject(Project project)
        {
            foreach (DataRow row in project.GetPgmsAsDataTable().Rows)
            {
                yield return new Pgm(project, row);
            }
            string pattern = string.Format("*{0}", FileExtension);
            foreach (FileInfo file in project.Location.EnumerateFiles(pattern, SearchOption.AllDirectories))
            {
                yield return new Pgm(project, file);
            }
        }

        private Project project;
        private int? id;
        private FileInfo file;

        public PgmSource Source { get; private set; }
        public string Name { get; private set; }
        public string Content { get; private set; }

        private Pgm(Project project, DataRow row)
        {
            this.project = project;
            id = row.Field<int>("ProgramId");
            Source = PgmSource.Database;
            Name = row.Field<string>("Name");
            Content = row.Field<string>("Content");
        }

        private Pgm(Project project, FileInfo file)
        {
            this.project = project;
            this.file = file;
            Source = PgmSource.File;
            Name = Path.GetFileNameWithoutExtension(file.Name);
            Content = File.ReadAllText(file.FullName);
        }

        public void Delete()
        {
            switch (Source)
            {
                case PgmSource.Database:
                    project.DeletePgm(id.Value);
                    break;
                case PgmSource.File:
                    file.Recycle();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
