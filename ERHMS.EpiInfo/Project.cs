using Epi;
using Epi.Data;
using ERHMS.Utility;
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

        public static Project Create(string name, string description, DirectoryInfo location, string driver, string connectionString)
        {
            string safeConnectionString = DataExtensions.ToSafeString(connectionString);
            Log.Current.DebugFormat("Creating project: {0}, {1}, {2}, {3}", name, location.FullName, driver, safeConnectionString);
            Project project = new Project
            {
                Name = name,
                Description = description,
                Location = location,
                CollectedDataDriver = driver,
                CollectedDataConnectionString = connectionString
            };
            project.CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = connectionString;
            project.CollectedData.Initialize(project.CollectedDataDbInfo, driver, true);
            project.CreateCanvasesTable();
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.Driver);
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
