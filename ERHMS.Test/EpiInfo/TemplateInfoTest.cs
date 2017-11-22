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
                string name = nameof(TemplateInfoTest) + index;
                string path = Path.Combine(configuration.Directories.Templates, level.ToDirectoryName(), name + TemplateInfo.FileExtension);
                Create(level, path);
                paths[level].Add(path);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        private void Create(TemplateLevel level, string path)
        {
            string resourceName = string.Format("ERHMS.Test.Resources.Sample.ADDFull.{0}.xml", level);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);
        }

        [Test]
        public void TryReadTest()
        {
            TryReadTest(TemplateLevel.Project);
            TryReadTest(TemplateLevel.View);
        }

        private void TryReadTest(TemplateLevel level)
        {
            using (TempFile file = new TempFile())
            {
                Create(level, file.FullName);
                TemplateInfo templateInfo;
                Assert.IsTrue(TemplateInfo.TryRead(file.FullName, out templateInfo));
                PropertiesTest(templateInfo, level);
            }
        }

        [Test]
        public void GetTest()
        {
            GetTest(TemplateLevel.Project);
            GetTest(TemplateLevel.View);
        }

        private void GetTest(TemplateLevel level)
        {
            using (TempFile file = new TempFile())
            {
                Create(level, file.FullName);
                PropertiesTest(TemplateInfo.Get(file.FullName), level);
            }
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
    }
}
