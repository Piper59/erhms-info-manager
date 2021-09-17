using ERHMS.Data;
using ERHMS.EpiInfo;

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
            ProjectInfo projectInfo = new ProjectInfo
            {
                Name = ProjectName,
                Description = ProjectDescription,
                Location = ProjectLocation
            };
            ProjectExtensions.Create(projectInfo, DatabaseProvider.ToDatabase(ConnectionString));
        }
    }
}
