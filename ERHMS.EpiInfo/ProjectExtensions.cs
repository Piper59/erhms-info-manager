using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Data;
using log4net;
using System;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ProjectExtensions
    {
        private static ILog Log { get; } = LogManager.GetLogger(nameof(ERHMS));

        public static Project Create(ProjectCreationInfo info)
        {
            Log.Debug($"Creating project: {info.FilePath}");
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
                    DBCnnStringBuilder = info.Database.ConnectionStringBuilder,
                    DBName = info.Database.Name
                }
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, project.CollectedDataDriver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        public static bool IsInitialized(this Project @this)
        {
            return @this.Metadata.TableExists("metaDbInfo");
        }

        public static void Initialize(this Project @this)
        {
            ((MetadataDbProvider)@this.Metadata).CreateMetadataTables();
        }
    }
}
