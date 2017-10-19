using ERHMS.EpiInfo;
using ERHMS.Utility;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using NUnit.Framework;
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
        public Project Project { get; private set; }

        public void SetUp()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string location = Path.Combine(Configuration.GetNewInstance().Directories.Project, "Sample");
            string projectPath = Path.Combine(location, "Sample" + Project.FileExtension);
            string databasePath = Path.ChangeExtension(projectPath, OleDbExtensions.FileExtensions.Access);
            Directory.CreateDirectory(location);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", projectPath);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.mdb", databasePath);
            ProjectInfo.Get(projectPath).SetAccessDatabase();
            Project = new Project(projectPath);
        }

        public void TearDown()
        {
            if (Directory.Exists(Project.Location))
            {
                Directory.Delete(Project.Location, true);
            }
        }
    }

    public class SqlServerSampleProjectCreator : ISampleProjectCreator
    {
        private SqlServerDatabaseCreator creator;
        private bool created;

        public Project Project { get; private set; }

        public SqlServerSampleProjectCreator()
        {
            creator = new SqlServerDatabaseCreator();
        }

        public void SetUp()
        {
            creator.SetUp();
            created = true;
            using (SqlConnection connection = new SqlConnection(creator.Builder.ConnectionString))
            {
                Server server = new Server(new ServerConnection(connection));
                string script = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.Sample.sql");
                server.ConnectionContext.ExecuteNonQuery(script);
            }
            string location = Path.Combine(Configuration.GetNewInstance().Directories.Project, "Sample");
            string path = Path.Combine(location, "Sample" + Project.FileExtension);
            Directory.CreateDirectory(location);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
            ProjectInfo.Get(path).SetDatabase(creator.Builder.ConnectionString, Configuration.SqlDriver);
            Project = new Project(path);
        }

        public void TearDown()
        {
            if (Directory.Exists(Project.Location))
            {
                Directory.Delete(Project.Location, true);
            }
            if (created)
            {
                creator.TearDown();
            }
            else
            {
                TestContext.Error.WriteLine("Database '{0}' may need to be manually dropped.", creator.Builder.InitialCatalog);
            }
        }
    }
}
