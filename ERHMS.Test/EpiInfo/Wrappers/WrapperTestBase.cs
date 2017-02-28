using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public abstract class WrapperTestBase
    {
        protected class WrapperEventCollection : List<WrapperEventArgs>
        {
            public WrapperEventCollection(Wrapper wrapper)
            {
                wrapper.Event += (sender, e) =>
                {
                    Add(e);
                };
            }
        }

        protected TempDirectory directory;
        protected Configuration configuration;
        protected Project project;
        protected Wrapper wrapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.Path).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            string location = Path.Combine(configuration.Directories.Project, "Sample");
            Directory.CreateDirectory(location);
            string projectPath = Path.Combine(location, "Sample.prj");
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.prj", projectPath);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.mdb", Path.ChangeExtension(projectPath, ".mdb"));
            ProjectInfo.Get(projectPath).SetAccessConnectionString();
            project = new Project(projectPath);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            if (wrapper != null)
            {
                if (!wrapper.Exited.WaitOne(10000))
                {
                    TestContext.Error.WriteLine("Wrapper is not responding.");
                }
                wrapper = null;
            }
        }
    }
}
