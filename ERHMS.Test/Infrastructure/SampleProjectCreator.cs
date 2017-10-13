using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using System.Data;
using System.IO;
using System.Reflection;

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
        public SqlServerProjectCreator Creator { get; private set; }

        public Project Project
        {
            get { return Creator.Project; }
        }

        public SqlServerSampleProjectCreator(string name)
        {
            Creator = new SqlServerProjectCreator(name);
        }

        public void SetUp()
        {
            Creator.SetUp();
            using (IDbConnection connection = Creator.Project.Driver.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.Sample.sql")));
            }
        }

        public void TearDown()
        {
            Creator.TearDown();
        }
    }
}
