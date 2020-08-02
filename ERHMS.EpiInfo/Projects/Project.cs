using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Data.Databases;
using System;
using System.IO;

namespace ERHMS.EpiInfo.Projects
{
    public class Project : Epi.Project
    {
        public new MetadataDbProvider Metadata => (MetadataDbProvider)base.Metadata;

        public static Project Create(ProjectCreationInfo info)
        {
            Log.Default.Debug($"Creating project: {info.FilePath}");
            Directory.CreateDirectory(info.Location);
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Name = info.Name,
                Description = info.Description,
                Location = info.Location,
                CollectedDataDriver = info.Database.Type.ToDriver(),
                CollectedDataConnectionString = info.Database.ConnectionString,
                CollectedDataDbInfo = new DbDriverInfo
                {
                    DBCnnStringBuilder = info.Database.GetConnectionStringBuilder(),
                    DBName = info.Database.Name
                }
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, project.CollectedDataDriver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        private Project() { }

        public Project(string path)
            : base(path) { }

        public bool IsInitialized()
        {
            return Metadata.TableExists("metaDbInfo");
        }

        public void Initialize()
        {
            Metadata.CreateMetadataTables();
        }
    }
}
