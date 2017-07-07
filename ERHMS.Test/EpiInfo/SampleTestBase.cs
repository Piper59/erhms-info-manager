using Epi;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo
{
    public class SampleTestBase
    {
        protected TempDirectory directory;
        protected Configuration configuration;
        protected Project project;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.FullName).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            string location = Path.Combine(configuration.Directories.Project, "Sample");
            Directory.CreateDirectory(location);
            string projectPath = Path.Combine(location, "Sample.prj");
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.prj", projectPath);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.Sample.mdb", Path.ChangeExtension(projectPath, ".mdb"));
            ProjectInfo.Get(projectPath).SetAccessConnectionString();
            project = new Project(projectPath);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }
    }
}
