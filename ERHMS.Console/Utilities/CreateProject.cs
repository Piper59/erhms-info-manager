using Epi;
using ERHMS.Data;
using ERHMS.EpiInfo;
using System;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class CreateProject : Utility
    {
        public DatabaseType DatabaseType { get; }
        public string ConnectionString { get; }
        public string ProjectLocation { get; }
        public string ProjectName { get; }

        public CreateProject(DatabaseType databaseType, string connectionString, string projectLocation, string projectName)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            ProjectLocation = projectLocation;
            ProjectName = projectName;
        }

        protected override void RunCore()
        {
            ProjectCreationInfo info = new ProjectCreationInfo
            {
                Location = ProjectLocation,
                Name = ProjectName
            };
            if (File.Exists(info.FilePath))
            {
                throw new ArgumentException("Project already exists.");
            }
            IDatabase database = DatabaseFactory.GetDatabase(DatabaseType, ConnectionString);
            if (database.Exists())
            {
                throw new ArgumentException("Database already exists.");
            }
            database.Create();
            info.Database = database;
            Project project = ProjectExtensions.Create(info);
            project.Initialize();
            Log.Default.Debug("Project has been created");
        }
    }
}
