using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.EpiInfo
{
    public partial class Project : Epi.Project
    {
        public new const string FileExtension = ".prj";

        public static Project Create(ProjectCreationInfo info)
        {
            Log.Current.DebugFormat("Creating project: {0}", info.ToString());
            info.Location.Create();
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Name = info.Name,
                Description = info.Description,
                Location = info.Location.FullName,
                CollectedDataDriver = info.Driver,
                CollectedDataConnectionString = info.Builder.ConnectionString
            };
            project.CollectedDataDbInfo = new DbDriverInfo
            {
                DBCnnStringBuilder = info.Builder,
                DBName = info.DatabaseName
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, info.Driver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.Driver);
            if (info.Initialize)
            {
                Log.Current.DebugFormat("Initializing project: {0}", project.FilePath);
                project.Metadata.CreateMetadataTables();
                project.Metadata.AddVersionColumn();
                project.SetVersion(Assembly.GetExecutingAssembly().GetVersion().ToString());
                project.Metadata.CreateCanvasesTable();
            }
            project.Save();
            return project;
        }

        public new MetadataDbProvider Metadata
        {
            get { return (MetadataDbProvider)base.Metadata; }
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
            : this(file.FullName) { }

        public bool IsValidViewName(string viewName, out InvalidViewNameReason reason)
        {
            if (ViewExtensions.IsValidName(viewName, out reason))
            {
                if (Views.Names.ContainsIgnoreCase(viewName))
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
            else
            {
                return false;
            }
        }

        public string SuggestViewName(string viewName)
        {
            return ViewExtensions.SanitizeName(viewName).MakeUnique("{0}_{1}", value => Views.Names.ContainsIgnoreCase(value));
        }

        public override void Save()
        {
            Log.Current.DebugFormat("Saving project: {0}", FilePath);
            base.Save();
        }
    }
}
