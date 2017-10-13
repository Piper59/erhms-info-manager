using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.EpiInfo
{
    public class TemplateInfoTest
    {
        private TempDirectory directory;
        private IDictionary<TemplateLevel, ICollection<string>> paths;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(TemplateInfoTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            paths = new Dictionary<TemplateLevel, ICollection<string>>();
            InitializePaths(configuration, TemplateLevel.Project);
            InitializePaths(configuration, TemplateLevel.View);
        }

        private void InitializePaths(Configuration configuration, TemplateLevel level)
        {
            paths[level] = new List<string>();
            for (int index = 0; index < 3; index++)
            {
                string directoryPath = Path.Combine(configuration.Directories.Templates, level.ToDirectoryName());
                paths[level].Add(Create(directoryPath, level, nameof(TemplateInfoTest) + index));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        private string Create(string directoryPath, TemplateLevel level, string name)
        {
            string resourceName = string.Format("ERHMS.Test.Resources.Sample.ADDFull.{0}.xml", level);
            string path = Path.Combine(directoryPath, name + TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);
            return path;
        }

        private void PropertiesTest(TemplateInfo templateInfo)
        {
            Assert.AreEqual("ADDFull", templateInfo.Name);
            Assert.AreEqual("Description for ADDFull template", templateInfo.Description);
        }

        private void PropertiesTest(TemplateInfo templateInfo, TemplateLevel level)
        {
            PropertiesTest(templateInfo);
            Assert.AreEqual(level, templateInfo.Level);
        }

        [Test]
        public void TryReadTest()
        {
            TryReadTest(TemplateLevel.Project);
            TryReadTest(TemplateLevel.View);
        }

        private void TryReadTest(TemplateLevel level)
        {
            string path = Create(directory.FullName, level, nameof(TryReadTest));
            TemplateInfo templateInfo;
            Assert.IsTrue(TemplateInfo.TryRead(path, out templateInfo));
            PropertiesTest(templateInfo, level);
        }

        [Test]
        public void GetTest()
        {
            GetTest(TemplateLevel.Project);
            GetTest(TemplateLevel.View);
        }

        private void GetTest(TemplateLevel level)
        {
            string path = Create(directory.FullName, level, nameof(GetTest));
            PropertiesTest(TemplateInfo.Get(path), level);
        }

        [Test]
        public void GetAllTest()
        {
            ICollection<TemplateInfo> templateInfos = TemplateInfo.GetAll().ToList();
            CollectionAssert.AreEquivalent(paths.SelectMany(pair => pair.Value), templateInfos.Select(templateInfo => templateInfo.FilePath));
            foreach (TemplateInfo templateInfo in templateInfos)
            {
                PropertiesTest(templateInfo);
            }
        }

        [Test]
        public void GetByLevelTest()
        {
            GetByLevelTest(TemplateLevel.Project);
            GetByLevelTest(TemplateLevel.View);
        }

        private void GetByLevelTest(TemplateLevel level)
        {
            ICollection<TemplateInfo> templateInfos = TemplateInfo.GetByLevel(level).ToList();
            CollectionAssert.AreEquivalent(paths[level], templateInfos.Select(templateInfo => templateInfo.FilePath));
            foreach (TemplateInfo templateInfo in templateInfos)
            {
                PropertiesTest(templateInfo, level);
            }
        }
    }
}
