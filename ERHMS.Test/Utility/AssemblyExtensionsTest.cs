using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.Utility
{
    public class AssemblyExtensionsTest
    {
        private const string ResourceName = "ERHMS.Test.Resources.Message.txt";
        private const string ResourceContent = "Hello, world!";

        [Test]
        public void GetEntryDirectoryPathTest()
        {
            Assert.AreEqual(Environment.CurrentDirectory, AssemblyExtensions.GetEntryDirectoryPath());
        }

        [Test]
        public void GetManifestResourceTextTest()
        {
            Assert.AreEqual(ResourceContent, Assembly.GetExecutingAssembly().GetManifestResourceText(ResourceName));
        }

        [Test]
        public void CopyManifestResourceToTest()
        {
            using (TempFile file = new TempFile())
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo(ResourceName, file.FullName);
                Assert.AreEqual(ResourceContent, File.ReadAllText(file.FullName));
            }
        }
    }
}
