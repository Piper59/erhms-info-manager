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
                paths.Add(Create(configuration.Directories.Project, nameof(ProjectInfoTest) + index));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        private string Create(string directoryPath, string name)
        {
            string path = Path.Combine(directoryPath, name + Project.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
            return path;
        }

        private void PropertiesTest(ProjectInfo projectInfo)
        {
            Assert.AreEqual(new Version(1, 0, 0, 0), projectInfo.Version);
            Assert.AreEqual("Sample", projectInfo.Name);
            Assert.AreEqual("Description for Sample project", projectInfo.Description);
        }

        [Test]
        public void TryReadTest()
        {
            string path = Create(directory.FullName, nameof(TryReadTest));
            ProjectInfo projectInfo;
            Assert.IsTrue(ProjectInfo.TryRead(path, out projectInfo));
            PropertiesTest(projectInfo);
        }

        [Test]
        public void GetTest()
        {
            string path = Create(directory.FullName, nameof(GetTest));
            PropertiesTest(ProjectInfo.Get(path));
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

        [Test]
        public void SetAccessDatabaseTest()
        {
            ProjectInfo projectInfo = ProjectInfo.Get(Create(directory.FullName, nameof(SetAccessDatabaseTest)));
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
