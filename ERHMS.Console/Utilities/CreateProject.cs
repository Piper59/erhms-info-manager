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
        public string ProjectDescription { get; }

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

        public CreateProject(
            DatabaseProvider databaseProvider,
            string connectionString,
            string projectLocation,
            string projectName,
            string projectDescription)
            : this(databaseProvider, connectionString, projectLocation, projectName)
        {
            ProjectDescription = projectDescription;
        }

        public void Run()
        {
            ProjectCreationInfo creationInfo = new ProjectCreationInfo
            {
                Name = ProjectName,
                Description = ProjectDescription,
                Location = ProjectLocation,
                Database = DatabaseProvider.ToDatabase(ConnectionString)
            };
            if (File.Exists(creationInfo.FilePath))
            {
                throw new InvalidOperationException("Project already exists.");
            }
            ProjectExtensions.Create(creationInfo);
        }
    }
}
