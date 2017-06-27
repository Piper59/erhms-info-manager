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
        public void EncryptSafeTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(EncryptSafeTest)))
            {
                ConfigurationExtensions.Create(directory.Path).Save();
                ConfigurationExtensions.Load();
                Assert.AreEqual("Hello, world!", Configuration.Decrypt(ConfigurationExtensions.EncryptSafe("Hello, world!")));
                Assert.IsNull(ConfigurationExtensions.EncryptSafe(null));
            }
        }

        [Test]
        public void DecryptSafeTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(EncryptSafeTest)))
            {
                ConfigurationExtensions.Create(directory.Path).Save();
                ConfigurationExtensions.Load();
                Assert.AreEqual("Hello, world!", ConfigurationExtensions.DecryptSafe(Configuration.Encrypt("Hello, world!")));
                Assert.IsNull(ConfigurationExtensions.DecryptSafe(null));
            }
        }

        [Test]
        public void CreateAndSaveTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(CreateAndSaveTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.Path);
                FileAssert.DoesNotExist(ConfigurationExtensions.FilePath);
                configuration.Save();
                FileAssert.Exists(ConfigurationExtensions.FilePath);
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
    }
}
