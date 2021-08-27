using ERHMS.Data;
using ERHMS.EpiInfo;
using System;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class CreateProject : Utility
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

        public override void Run()
        {
            ProjectCreationInfo projectCreationInfo = new ProjectCreationInfo
            {
                Name = ProjectName,
                Description = ProjectDescription,
                Location = ProjectLocation,
                Database = DatabaseProvider.ToDatabase(ConnectionString)
            };
            ProjectExtensions.Create(projectCreationInfo);
        }
    }
}
