using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
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
            ConfigurationExtensions.Create(AssemblyExtensions.GetEntryDirectoryPath()).Save();
            Configuration = ConfigurationExtensions.Load();
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
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
