using Epi;
using ERHMS.Data;
using ERHMS.EpiInfo;
using System;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class CreateProject : IUtility
    {
        public DatabaseProvider DatabaseProvider { get; }
        public string ConnectionString { get; }
        public string ProjectLocation { get; }
        public string ProjectName { get; }

        public CreateProject(
            DatabaseProvider databaseProvider,
            string connectionString,
            string projectLocation,
            string projectName)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
            ProjectLocation = projectLocation;
            ProjectName = projectName;
        }

        public void Run()
        {
            ProjectCreationInfo creationInfo = new ProjectCreationInfo
            {
                Name = ProjectName,
                Location = ProjectLocation
            };
            if (File.Exists(creationInfo.FilePath))
            {
                throw new InvalidOperationException("Project already exists.");
            }
            IDatabase database = DatabaseProvider.ToDatabase(ConnectionString);
            if (database.Exists())
            {
                throw new InvalidOperationException("Database already exists.");
            }
            database.Create();
            creationInfo.Database = database;
            Project project = ProjectExtensions.Create(creationInfo);
            project.Initialize();
        }
    }
}
