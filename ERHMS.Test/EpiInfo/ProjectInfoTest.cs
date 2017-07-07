using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ERHMS.Test.EpiInfo
{
    public class ProjectInfoTest
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(ProjectInfoTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            ConfigurationExtensions.Load();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void TryReadTest()
        {
            string path = directory.CombinePaths("TryReadTest.prj");
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
            ProjectInfo projectInfo;
            Assert.IsTrue(ProjectInfo.TryRead(path, out projectInfo));
            Assert.AreEqual(new Version(1, 0, 0, 0), projectInfo.Version);
            Assert.AreEqual("Sample", projectInfo.Name);
            Assert.AreEqual("Description for Sample project", projectInfo.Description);
        }

        [Test]
        public void SetAccessConnectionStringTest()
        {
            string path = directory.CombinePaths("SetAccessConnectionStringTest.prj");
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", path);
            ProjectInfo projectInfo = ProjectInfo.Get(path);
            projectInfo.SetAccessConnectionString();
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlElement databaseElement = document.SelectSingleElement("/Project/CollectedData/Database");
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                ConnectionString = Configuration.Decrypt(databaseElement.GetAttribute("connectionString"))
            };
            Assert.AreEqual(Path.ChangeExtension(path, ".mdb"), builder.DataSource);
        }
    }
}
