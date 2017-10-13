using Epi;
using ERHMS.EpiInfo;
using NUnit.Framework;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test
{
    public interface IProjectCreator
    {
        ProjectCreationInfo Info { get; }
        Project Project { get; }

        void SetUp();
        void TearDown();
    }

    public class AccessProjectCreator : IProjectCreator
    {
        public AccessDatabaseCreator Creator { get; private set; }
        public ProjectCreationInfo Info { get; private set; }
        public Project Project { get; private set; }

        public AccessProjectCreator(string name)
        {
            Creator = new AccessDatabaseCreator(name);
            Info = new ProjectCreationInfo
            {
                Name = name,
                Location = Creator.Directory.FullName,
                Driver = Configuration.AccessDriver,
                Builder = Creator.Builder,
                DatabaseName = name,
                Initialize = true
            };
        }

        public void SetUp()
        {
            Creator.SetUp();
            Project = Project.Create(Info);
        }

        public void TearDown()
        {
            Creator.TearDown();
        }
    }

    public class SqlServerProjectCreator : IProjectCreator
    {
        private bool created;

        public TempDirectory Directory { get; private set; }
        public SqlServerDatabaseCreator Creator { get; private set; }
        public ProjectCreationInfo Info { get; private set; }
        public Project Project { get; private set; }

        public SqlServerProjectCreator(string name)
        {
            Directory = new TempDirectory(name);
            Creator = new SqlServerDatabaseCreator();
            Info = new ProjectCreationInfo
            {
                Name = name,
                Location = Directory.FullName,
                Driver = Configuration.SqlDriver,
                Builder = Creator.Builder,
                DatabaseName = Creator.Builder.InitialCatalog,
                Initialize = true
            };
        }

        public void SetUp()
        {
            Creator.SetUp();
            created = true;
            Project = Project.Create(Info);
        }

        public void TearDown()
        {
            if (created)
            {
                Directory.Dispose();
                Creator.TearDown();
            }
            else
            {
                TestContext.Error.WriteLine("Database '{0}' may need to be manually dropped.", Creator.Builder.InitialCatalog);
            }
        }
    }
}
