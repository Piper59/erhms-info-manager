using ERHMS.EpiInfo;
using ERHMS.Utility;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Configuration = Epi.Configuration;

namespace ERHMS.Test
{
    public interface ISampleProjectCreator
    {
        Project Project { get; }

        void SetUp();
        void TearDown();
    }

    public class AccessSampleProjectCreator : ISampleProjectCreator
    {
        public TempDirectory Directory { get; private set; }
        public Project Project { get; private set; }

        public AccessSampleProjectCreator(string name)
        {
            Directory = new TempDirectory(name);
        }

        public void SetUp()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string projectPath = Directory.CombinePaths("Sample" + Project.FileExtension);
            string databasePath = Path.ChangeExtension(projectPath, OleDbExtensions.FileExtensions.Access);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", projectPath);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.mdb", databasePath);
            ProjectInfo.Get(projectPath).SetAccessDatabase();
            Project = new Project(projectPath);
        }

        public void TearDown()
        {
            Directory.Dispose();
        }
    }

    public class SqlServerSampleProjectCreator : ISampleProjectCreator
    {
        public TempDirectory Directory { get; private set; }
        public SqlServerDatabaseCreator Creator { get; private set; }
        public Project Project { get; private set; }

        public SqlServerSampleProjectCreator(string name)
        {
            Directory = new TempDirectory(name);
            Creator = new SqlServerDatabaseCreator();
        }

        public void SetUp()
        {
            Creator.SetUp();
            using (SqlConnection connection = new SqlConnection(Creator.Builder.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection));
                string script = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.Sample.sql");
                server.ConnectionContext.ExecuteNonQuery(script);
            }
            string path = Directory.CombinePaths("Sample" + Project.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
            ProjectInfo.Get(path).SetDatabase(Creator.Builder.ConnectionString, Configuration.SqlDriver);
            Project = new Project(path);
        }

        public void TearDown()
        {
            Directory.Dispose();
            Creator.TearDown();
        }
    }
}
