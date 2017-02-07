using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public partial class Project : Epi.Project
    {
        public new const string FileExtension = ".prj";

        public static Project Create(ProjectCreationInfo creationInfo)
        {
            Log.Current.DebugFormat("Creating project: {0}", creationInfo.ToString());
            creationInfo.Location.Create();
            Project project = new Project
            {
                Version = Assembly.GetExecutingAssembly().GetVersion(),
                Name = creationInfo.Name,
                Description = creationInfo.Description,
                Location = creationInfo.Location.FullName,
                CollectedDataDriver = creationInfo.Driver,
                CollectedDataConnectionString = creationInfo.Builder.ConnectionString
            };
            project.Id = project.GetProjectId();
            project.CollectedDataDbInfo = new DbDriverInfo
            {
                DBCnnStringBuilder = creationInfo.Builder,
                DBName = creationInfo.DatabaseName
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, creationInfo.Driver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.Driver);
            if (creationInfo.Initialize)
            {
                Log.Current.DebugFormat("Initializing project: {0}", project.FilePath);
                project.Metadata.CreateMetadataTables();
                project.Metadata.CreateCanvasesTable();
            }
            project.Save();
            return project;
        }

        private XmlElement XmlElement
        {
            get { return GetXmlDocument().DocumentElement; }
        }

        public Version Version
        {
            get
            {
                Version version;
                if (Version.TryParse(XmlElement.GetAttribute("erhmsVersion"), out version))
                {
                    return version;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                XmlElement.SetAttribute("erhmsVersion", value.ToString());
            }
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
                if (Views.Contains(viewName))
                {
                    reason = InvalidViewNameReason.ViewExists;
                    return false;
                }
                else if (Driver.TableExists(viewName))
                {
                    reason = InvalidViewNameReason.TableExists;
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
            return ViewExtensions.SanitizeName(viewName).MakeUnique("{0}_{1}", value => Views.Contains(value) || Driver.TableExists(value));
        }

        public override void Save()
        {
            Log.Current.DebugFormat("Saving project: {0}", FilePath);
            base.Save();
        }
    }
}
