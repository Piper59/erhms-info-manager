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

        private void CreateAndLoadConfiguration(string userDirectoryPath, bool isFipsCryptoRequired)
        {
            Configuration configuration = ConfigurationExtensions.Create(userDirectoryPath, isFipsCryptoRequired);
            configuration.Save();
            ConfigurationExtensions.Load();
        }

        [Test]
        public void EncryptSafeTest()
        {
            EncryptSafeTest(false);
            EncryptSafeTest(true);
        }

        private void EncryptSafeTest(bool isFipsCryptoRequired)
        {
            using (TempDirectory directory = new TempDirectory(nameof(EncryptSafeTest) + isFipsCryptoRequired))
            {
                CreateAndLoadConfiguration(directory.FullName, isFipsCryptoRequired);
                Assert.IsNull(ConfigurationExtensions.EncryptSafe(null));
                EncryptSafeTest("");
                EncryptSafeTest("Hello, world!");
            }
        }

        private void EncryptSafeTest(string value)
        {
            Assert.AreEqual(value, Configuration.Decrypt(ConfigurationExtensions.EncryptSafe(value)));
        }

        [Test]
        public void DecryptSafeTest()
        {
            DecryptSafeTest(false);
            DecryptSafeTest(true);
        }

        private void DecryptSafeTest(bool isFipsCryptoRequired)
        {
            using (TempDirectory directory = new TempDirectory(nameof(DecryptSafeTest) + isFipsCryptoRequired))
            {
                CreateAndLoadConfiguration(directory.FullName, isFipsCryptoRequired);
                Assert.IsNull(ConfigurationExtensions.DecryptSafe(null));
                DecryptSafeTest("");
                DecryptSafeTest("Hello, world!");
            }
        }

        private void DecryptSafeTest(string value)
        {
            Assert.AreEqual(value, ConfigurationExtensions.DecryptSafe(Configuration.Encrypt(value)));
        }

        [Test]
        public void CreateAndSaveTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(CreateAndSaveTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.FullName);
                FileAssert.DoesNotExist(ConfigurationExtensions.FilePath);
                configuration.Save();
                FileAssert.Exists(ConfigurationExtensions.FilePath);
                XmlDocument document = new XmlDocument();
                document.Load(ConfigurationExtensions.FilePath);
                XmlNode directoriesNode = document.SelectSingleNode("/Config/Directories");
                StringAssert.Contains(directory.FullName, directoriesNode.SelectSingleNode("Project").InnerText);
                StringAssert.Contains(directory.FullName, directoriesNode.SelectSingleNode("Templates").InnerText);
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
                ConfigurationExtensions.Create(directory.FullName).Save();
                Assert.IsTrue(ConfigurationExtensions.TryLoad(out configuration));
                StringAssert.Contains(directory.FullName, configuration.Directories.Project);
                StringAssert.Contains(directory.FullName, configuration.Directories.Templates);
            }
        }

        [Test]
        public void SetUserDirectoriesTest()
        {
            using (TempDirectory directory1 = new TempDirectory(nameof(SetUserDirectoriesTest) + 1))
            using (TempDirectory directory2 = new TempDirectory(nameof(SetUserDirectoriesTest) + 2))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory1.FullName);
                StringAssert.DoesNotContain(directory2.FullName, configuration.Directories.Project);
                StringAssert.DoesNotContain(directory2.FullName, configuration.Directories.Templates);
                configuration.SetUserDirectories(directory2.FullName);
                StringAssert.Contains(directory2.FullName, configuration.Directories.Project);
                StringAssert.Contains(directory2.FullName, configuration.Directories.Templates);
            }
        }

        [Test]
        public void CreateUserDirectoriesTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(CreateUserDirectoriesTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.FullName);
                configuration.CreateUserDirectories();
                DirectoryAssert.Exists(configuration.Directories.Project);
                DirectoryAssert.Exists(configuration.Directories.Templates);
            }
        }

        [Test]
        public void GetRootPathTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(GetRootPathTest)))
            {
                Configuration configuration = ConfigurationExtensions.Create(directory.FullName);
                Assert.AreEqual(directory.FullName, configuration.GetRootPath());
            }
        }
    }
}
