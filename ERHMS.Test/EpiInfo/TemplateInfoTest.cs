using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.EpiInfo
{
    public class TemplateInfoTest
    {
        [Test]
        public void TryReadTest()
        {
            FileInfo file = new FileInfo(Path.GetTempFileName());
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.EpiInfo.Surveillance.xml", file);
                TemplateInfo templateInfo;
                Assert.IsTrue(TemplateInfo.TryRead(file, out templateInfo));
                Assert.AreEqual("Surveillance", templateInfo.Name);
                Assert.AreEqual("Description for Surveillance.xml", templateInfo.Description);
                Assert.AreEqual(TemplateLevel.View, templateInfo.Level);
            }
            finally
            {
                file.Delete();
            }
        }
    }
}
