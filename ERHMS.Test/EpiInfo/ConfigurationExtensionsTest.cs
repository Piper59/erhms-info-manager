using Epi;
using ERHMS.EpiInfo;
using NUnit.Framework;
using System.IO;
using System.Xml;

namespace ERHMS.Test.EpiInfo
{
    public class ConfigurationExtensionsTest
    {
        [TearDown]
        public void TearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
        }

        [Test]
        public void CreateAndSaveTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(CreateAndSaveTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.Path);
                configuration.Save();
                FileInfo file = new FileInfo(ConfigurationExtensions.FilePath);
                FileAssert.Exists(file);
                Assert.IsTrue(file.CreationTime.IsRecent());
                XmlDocument document = new XmlDocument();
                document.Load(ConfigurationExtensions.FilePath);
                XmlNode directoriesNode = document.SelectSingleNode("/Config/Directories");
                StringAssert.Contains(directory.Path, directoriesNode.SelectSingleNode("Project").InnerText);
                StringAssert.Contains(directory.Path, directoriesNode.SelectSingleNode("Templates").InnerText);
            }
        }

        [Test]
        public void TryLoadTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(TryLoadTest)))
            {
                Configuration configuration;
                Assert.IsFalse(ConfigurationExtensions.TryLoad(out configuration));
                Assert.IsNull(configuration);
                ConfigurationExtensions.Create(directory.Path).Save();
                Assert.IsTrue(ConfigurationExtensions.TryLoad(out configuration));
                StringAssert.Contains(directory.Path, configuration.Directories.Project);
                StringAssert.Contains(directory.Path, configuration.Directories.Templates);
            }
        }

        [Test]
        public void CreateUserDirectoriesTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(CreateUserDirectoriesTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.Path);
                configuration.CreateUserDirectories();
                DirectoryAssert.Exists(configuration.Directories.Project);
                DirectoryAssert.Exists(configuration.Directories.Templates);
            }
        }

        [Test]
        public void ChangeUserDirectoriesTest()
        {
            using (TempDirectory directory1 = new TempDirectory(nameof(ChangeUserDirectoriesTest) + "1"))
            using (TempDirectory directory2 = new TempDirectory(nameof(ChangeUserDirectoriesTest) + "2"))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory1.Path);
                configuration.CreateUserDirectories();
                directory1.CreateFile("Projects", "Test", "Test.prj");
                directory1.CreateFile("Templates", "Forms", "Test.xml");
                configuration.ChangeUserDirectories(directory2.Path);
                FileAssert.Exists(directory2.CombinePaths("Projects", "Test", "Test.prj"));
                FileAssert.Exists(directory2.CombinePaths("Templates", "Forms", "Test.xml"));
            }
        }
    }
}
