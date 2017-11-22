using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo
{
    public class ProjectInfoTest
    {
        private TempDirectory directory;
        private ICollection<string> paths;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(ProjectInfoTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            paths = new List<string>();
            for (int index = 0; index < 3; index++)
            {
                string name = nameof(ProjectInfoTest) + index;
                DirectoryInfo directory = Directory.CreateDirectory(Path.Combine(configuration.Directories.Project, name));
                string path = Path.Combine(directory.FullName, name + Project.FileExtension);
                Create(path);
                paths.Add(path);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        private void Create(string path)
        {
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
        }

        [Test]
        public void TryReadTest()
        {
            using (TempFile file = new TempFile())
            {
                Create(file.FullName);
                ProjectInfo projectInfo;
                Assert.IsTrue(ProjectInfo.TryRead(file.FullName, out projectInfo));
                PropertiesTest(projectInfo);
            }
        }

        [Test]
        public void GetTest()
        {
            using (TempFile file = new TempFile())
            {
                Create(file.FullName);
                PropertiesTest(ProjectInfo.Get(file.FullName));
            }
        }

        [Test]
        public void GetAllTest()
        {
            ICollection<ProjectInfo> projectInfos = ProjectInfo.GetAll().ToList();
            CollectionAssert.AreEquivalent(paths, projectInfos.Select(projectInfo => projectInfo.FilePath));
            foreach (ProjectInfo projectInfo in projectInfos)
            {
                PropertiesTest(projectInfo);
            }
        }

        private void PropertiesTest(ProjectInfo projectInfo)
        {
            Assert.AreEqual(new Version(1, 0, 0, 0), projectInfo.Version);
            Assert.AreEqual("Sample", projectInfo.Name);
            Assert.AreEqual("Description for Sample project", projectInfo.Description);
        }

        [Test]
        public void SetAccessDatabaseTest()
        {
            using (TempFile file = new TempFile())
            {
                Create(file.FullName);
                ProjectInfo projectInfo = ProjectInfo.Get(file.FullName);
                projectInfo.SetAccessDatabase();
                XmlDocument document = new XmlDocument();
                document.Load(projectInfo.FilePath);
                XmlElement element = document.SelectSingleElement("/Project/CollectedData/Database");
                string connectionString = Configuration.Decrypt(element.GetAttribute("connectionString"));
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connectionString);
                Assert.AreEqual(Path.ChangeExtension(projectInfo.FilePath, OleDbExtensions.FileExtensions.Access), builder.DataSource);
                Assert.AreEqual(Configuration.AccessDriver, element.GetAttribute("dataDriver"));
            }
        }
    }
}
