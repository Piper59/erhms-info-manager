using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo
{
    public partial class Project : Epi.Project
    {
        public new const string FileExtension = ".prj";
        private static readonly Regex InvalidViewNameCharacter = new Regex(@"[^a-zA-Z0-9_]");

        public static string SanitizeViewName(string viewName)
        {
            return InvalidViewNameCharacter.Replace(viewName, "");
        }

        public static Project Create(string name, string description, DirectoryInfo location, string driver, DbConnectionStringBuilder builder, string databaseName)
        {
            Log.Current.DebugFormat(
                "Creating project: {0}, {1}, {2}, {3}, {4}",
                name,
                location.FullName,
                driver,
                builder.ToSafeString(),
                databaseName);
            location.Create();
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Location = location,
                CollectedDataDriver = driver,
                CollectedDataConnectionString = builder.ConnectionString
            };
            project.CollectedDataDbInfo = new DbDriverInfo
            {
                DBCnnStringBuilder = builder,
                DBName = databaseName
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, driver, true);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.Driver);
            project.Metadata.CreateMetadataTables();
            project.Metadata.CreateCanvasesTable();
            project.Save();
            return project;
        }

        public new DirectoryInfo Location
        {
            get { return new DirectoryInfo(base.Location); }
            set { base.Location = value.FullName; }
        }

        public FileInfo File
        {
            get { return new FileInfo(FilePath); }
        }

        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        public new MetadataDbProvider Metadata
        {
            get { return (MetadataDbProvider)base.Metadata; }
        }

        private Project() { }

        public Project(string path)
            : base(path)
        {
            Log.Current.DebugFormat("Opening project: {0}", path);
        }

        public Project(FileInfo file)
            : this(file.FullName)
        { }

        public bool IsValidViewName(string viewName, out InvalidViewNameReason reason)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                reason = InvalidViewNameReason.Empty;
                return false;
            }
            else if (InvalidViewNameCharacter.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidCharacter;
                return false;
            }
            else if (!char.IsLetter(viewName.First()))
            {
                reason = InvalidViewNameReason.InvalidFirstCharacter;
                return false;
            }
            else if (Views.Names.Contains(viewName))
            {
                reason = InvalidViewNameReason.Duplicate;
                return false;
            }
            else
            {
                reason = InvalidViewNameReason.None;
                return true;
            }
        }

        public string SuggestViewName(string viewName)
        {
            string baseViewName = SanitizeViewName(viewName);
            if (!Views.Names.Contains(baseViewName))
            {
                return baseViewName;
            }
            else
            {
                for (int copy = 2; ; copy++)
                {
                    string copyViewName = string.Format("{0}_{1}", baseViewName, copy);
                    if (!Views.Names.Contains(copyViewName))
                    {
                        return copyViewName;
                    }
                }
            }
        }

        public override void Save()
        {
            Log.Current.DebugFormat("Saving project: {0}", File.FullName);
            base.Save();
        }
    }
}
