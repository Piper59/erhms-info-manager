using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public abstract class WrapperTest
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

        private TempDirectory directory;
        private ISampleProjectCreator creator;

        protected Configuration Configuration { get; private set; }
        protected Wrapper Wrapper { get; set; }

        protected Project Project
        {
            get { return creator.Project; }
        }

        protected abstract ISampleProjectCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration = ConfigurationExtensions.Load();
            Configuration.CreateUserDirectories();
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            if (Wrapper != null)
            {
                if (!Wrapper.Exited.WaitOne(TimeSpan.FromSeconds(10.0)))
                {
                    TestContext.Error.WriteLine("Wrapper is not responding.");
                }
                Wrapper = null;
            }
        }
    }
}
