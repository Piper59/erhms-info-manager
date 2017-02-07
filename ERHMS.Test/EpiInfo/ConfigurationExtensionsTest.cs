using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Xml;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo
{
    public class ConfigurationExtensionsTest
    {
        [Test]
        public void CreateAndSaveTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => CreateAndSaveTest());
            try
            {
                Settings.Default.RootDirectory = directory.FullName;
                Configuration configuration = ConfigurationExtensions.Create();
                configuration.Save();
                FileInfo file = new FileInfo(Configuration.DefaultConfigurationPath);
                FileAssert.Exists(file);
                Assert.IsTrue(file.LastWriteTime.IsRecent());
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
                        StringAssert.Contains(directory.FullName, element.InnerText);
                        DirectoryAssert.DoesNotExist(element.InnerText);
                    }
                }
            }
            finally
            {
                Settings.Default.Reset();
                File.Delete(Configuration.DefaultConfigurationPath);
                directory.Delete(true);
            }
        }

        [Test]
        public void LoadTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => LoadTest());
            try
            {
                Settings.Default.RootDirectory = directory.FullName;
                Assert.AreEqual(Log.GetDefaultDirectory().FullName, Log.GetDirectory().FullName);
                Configuration configuration = LoadTest(false);
                Assert.AreEqual(configuration.Directories.LogDir, Log.GetDirectory().FullName);
                LoadTest(true);
                FileInfo file = directory.GetFile("configuration.xml");
                File.Copy(Configuration.DefaultConfigurationPath, file.FullName);
                Settings.Default.ConfigurationFile = file.FullName;
                File.Delete(Configuration.DefaultConfigurationPath);
                LoadTest(true);
                FileAssert.AreEqual(file.FullName, Configuration.DefaultConfigurationPath);
            }
            finally
            {
                Settings.Default.Reset();
                File.Delete(Configuration.DefaultConfigurationPath);
                Log.SetDirectory(Log.GetDefaultDirectory());
                directory.Delete(true);
            }
        }

        private Configuration LoadTest(bool expected)
        {
            bool actual;
            Configuration configuration = ConfigurationExtensions.Load(out actual);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(Settings.Default.ConfigurationFile, Configuration.DefaultConfigurationPath);
            return configuration;
        }

        [Test]
        public void CreateAssetsTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => CreateAssetsTest());
            try
            {
                CreateAsset("Projects", "Test", "Test.prj");
                CreateAsset("Templates", "Forms", "Test.xml");
                Settings.Default.RootDirectory = directory.FullName;
                Configuration configuration = ConfigurationExtensions.Load();
                configuration.CreateAssets();
                AssetTest("Projects", "Test", "Test.prj");
                AssetTest("Templates", "Forms", "Test.xml");
                AssetTest("LICENSE.txt");
                AssetTest("NOTICE.txt");
            }
            finally
            {
                Settings.Default.Reset();
                File.Delete(Configuration.DefaultConfigurationPath);
                Log.SetDirectory(Log.GetDefaultDirectory());
                directory.Delete(true);
                DeleteAssets("Projects");
                DeleteAssets("Templates");
            }
        }

        private void CreateAsset(params string[] paths)
        {
            new FileInfo(Path.Combine(paths)).Touch();
        }

        private void AssetTest(params string[] paths)
        {
            FileAssert.Exists(Path.Combine(Settings.Default.RootDirectory, Path.Combine(paths)));
        }

        private void DeleteAssets(string directoryName)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryName);
            if (directory.Exists)
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
                DirectoryInfo subdirectory1 = directory.GetSubdirectory("1");
                DirectoryInfo subdirectory2 = directory.GetSubdirectory("2");
                Settings.Default.RootDirectory = subdirectory1.FullName;
                Configuration configuration = ConfigurationExtensions.Create();
                configuration.ChangeRoot(subdirectory2);
                configuration.Save();
                XmlDocument document = new XmlDocument();
                document.Load(Configuration.DefaultConfigurationPath);
                foreach (XmlElement element in document.SelectElements("/Config/Directories/*"))
                {
                    if (element.Name != "Configuration" && element.Name != "Working")
                    {
                        StringAssert.Contains(subdirectory2.FullName, element.InnerText);
                        DirectoryAssert.DoesNotExist(element.InnerText);
                    }
                }
            }
            finally
            {
                Settings.Default.Reset();
                File.Delete(Configuration.DefaultConfigurationPath);
                directory.Delete(true);
            }
        }
    }
}
