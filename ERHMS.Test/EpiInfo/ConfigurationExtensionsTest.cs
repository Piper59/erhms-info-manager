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

        private void CreateAndLoadConfiguration(string userDirectoryPath, bool fips)
        {
            Configuration configuration = ConfigurationExtensions.Create(userDirectoryPath);
            configuration.SetFipsCrypto(fips);
            configuration.Save();
            ConfigurationExtensions.Load();
        }

        [Test]
        public void EncryptSafeTest()
        {
            EncryptSafeTest(false);
            EncryptSafeTest(true);
        }

        private void EncryptSafeTest(bool fips)
        {
            using (TempDirectory directory = new TempDirectory(nameof(EncryptSafeTest) + (fips ? "Fips" : "")))
            {
                CreateAndLoadConfiguration(directory.FullName, fips);
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

        private void DecryptSafeTest(bool fips)
        {
            using (TempDirectory directory = new TempDirectory(nameof(DecryptSafeTest) + (fips ? "Fips" : "")))
            {
                CreateAndLoadConfiguration(directory.FullName, fips);
                Assert.IsNull(ConfigurationExtensions.EncryptSafe(null));
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
    }
}
