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

        public override void Run()
        {
            ProjectInfo projectInfo = new ProjectInfo
            {
                Name = ProjectName,
                Location = ProjectLocation
            };
            ProjectExtensions.Create(projectInfo, DatabaseProvider.ToDatabase(ConnectionString));
        }
    }
}
