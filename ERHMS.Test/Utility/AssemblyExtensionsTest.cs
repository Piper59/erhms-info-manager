using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.Utility
{
    public class AssemblyExtensionsTest
    {
        private const string ResourceName = "ERHMS.Test.Utility.Message.txt";
        private const string ResourceContent = "Hello, world!";

        [Test]
        public void GetManifestResourceTextTest()
        {
            Assert.AreEqual(Assembly.GetExecutingAssembly().GetManifestResourceText(ResourceName), ResourceContent);
        }

        [Test]
        public void CopyManifestResourceToTest()
        {
            FileInfo file = new FileInfo(Path.GetTempFileName());
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo(ResourceName, file);
                Assert.AreEqual(File.ReadAllText(file.FullName), ResourceContent);
            }
            finally
            {
                file.Delete();
            }
        }
    }
}
