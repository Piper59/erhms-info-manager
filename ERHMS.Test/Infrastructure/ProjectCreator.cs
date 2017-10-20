using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
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
        private AccessDatabaseCreator creator;

        public ProjectCreationInfo Info { get; private set; }
        public Project Project { get; private set; }

        public AccessProjectCreator(string name)
        {
            string location = Path.Combine(Configuration.GetNewInstance().Directories.Project, name);
            creator = AccessDatabaseCreator.ForPath(Path.Combine(location, name + OleDbExtensions.FileExtensions.Access));
            Info = new ProjectCreationInfo
            {
                Name = name,
                Location = location,
                Driver = Configuration.AccessDriver,
                Builder = creator.Builder,
                DatabaseName = name,
                Initialize = true
            };
        }

        public void SetUp()
        {
            Directory.CreateDirectory(Info.Location);
            creator.SetUp();
            Project = Project.Create(Info);
        }

        public void TearDown()
        {
            if (Directory.Exists(Project.Location))
            {
                Directory.Delete(Project.Location, true);
            }
        }
    }

    public class SqlServerProjectCreator : IProjectCreator
    {
        private SqlServerDatabaseCreator creator;
        private bool created;

        public ProjectCreationInfo Info { get; private set; }
        public Project Project { get; private set; }

        public SqlServerProjectCreator(string name)
        {
            creator = new SqlServerDatabaseCreator(name);
            Info = new ProjectCreationInfo
            {
                Name = name,
                Location = Path.Combine(Configuration.GetNewInstance().Directories.Project, name),
                Driver = Configuration.SqlDriver,
                Builder = creator.Builder,
                DatabaseName = creator.Builder.InitialCatalog,
                Initialize = true
            };
        }

        public void SetUp()
        {
            creator.SetUp();
            created = true;
            Project = Project.Create(Info);
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
