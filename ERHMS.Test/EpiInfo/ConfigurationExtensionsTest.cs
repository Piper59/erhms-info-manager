using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Xml;

namespace ERHMS.Test.EpiInfo
{
    public class ConfigurationExtensionsTest
    {
        private void DirectoriesTest(DirectoryInfo root)
        {
            FileInfo file = new FileInfo(Configuration.DefaultConfigurationPath);
            XmlDocument document = new XmlDocument();
            document.Load(file.FullName);
            foreach (XmlElement element in document.SelectElements("/Config/Directories/*"))
            {
                if (element.Name == "Configuration")
                {
                    Assert.AreEqual(file.DirectoryName, element.InnerText);
                }
                else if (element.Name != "Working")
                {
                    StringAssert.Contains(root.FullName, element.InnerText);
                }
            }
        }

        [Test]
        public void CreateAndSaveTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => CreateAndSaveTest());
            try
            {
                Configuration configuration = ConfigurationExtensions.Create(directory);
                configuration.Save();
                FileInfo file = new FileInfo(Configuration.DefaultConfigurationPath);
                FileAssert.Exists(file);
                Assert.IsTrue(file.LastWriteTime.IsRecent());
                DirectoriesTest(directory);
            }
            finally
            {
                File.Delete(Configuration.DefaultConfigurationPath);
                directory.Delete(true);
            }
        }

        [Test]
        public void TryLoadTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => TryLoadTest());
            try
            {
                Assert.AreEqual(Log.GetDefaultDirectory().FullName, Log.GetDirectory().FullName);
                Configuration configuration;
                Assert.IsFalse(ConfigurationExtensions.TryLoad(out configuration));
                Assert.IsNull(configuration);
                ConfigurationExtensions.Create(directory).Save();
                Assert.IsTrue(ConfigurationExtensions.TryLoad(out configuration));
                Assert.AreEqual(configuration.Directories.LogDir, Log.GetDirectory().FullName);
            }
            finally
            {
                File.Delete(Configuration.DefaultConfigurationPath);
                Log.SetDirectory(Log.GetDefaultDirectory());
                directory.Delete(true);
            }
        }

        [Test]
        public void CreateDirectoriesTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => CreateDirectoriesTest());
            try
            {
                Configuration configuration = ConfigurationExtensions.Create(directory);
                configuration.CreateDirectories();
                DirectoryAssert.Exists(configuration.Directories.Archive);
                DirectoryAssert.Exists(configuration.Directories.LogDir);
                DirectoryAssert.Exists(configuration.Directories.Output);
                DirectoryAssert.Exists(configuration.Directories.Project);
                DirectoryAssert.Exists(configuration.Directories.Samples);
                DirectoryAssert.Exists(configuration.Directories.Templates);
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Test]
        public void ChangeRootTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => ChangeRootTest());
            try
            {
                DirectoryInfo subdirectory1 = directory.GetDirectory("1");
                DirectoryInfo subdirectory2 = directory.GetDirectory("2");
                Configuration configuration = ConfigurationExtensions.Create(subdirectory1);
                configuration.ChangeRoot(subdirectory2);
                configuration.Save();
                DirectoriesTest(subdirectory2);
            }
            finally
            {
                File.Delete(Configuration.DefaultConfigurationPath);
                directory.Delete(true);
            }
        }
    }
}
