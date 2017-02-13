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
        public void GetEntryDirectoryPath()
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
            string path = Path.GetTempFileName();
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo(ResourceName, path);
                Assert.AreEqual(ResourceContent, File.ReadAllText(path));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
