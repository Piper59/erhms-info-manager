using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.EpiInfo
{
    public class ProjectInfoTest
    {
        [Test]
        public void TryReadTest()
        {
            string path = Path.GetTempFileName();
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.prj", path);
                ProjectInfo projectInfo;
                Assert.IsTrue(ProjectInfo.TryRead(path, out projectInfo));
                Assert.AreEqual(new Version(1, 0, 0, 0), projectInfo.Version);
                Assert.AreEqual("Sample", projectInfo.Name);
                Assert.AreEqual("Description for Sample project", projectInfo.Description);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
