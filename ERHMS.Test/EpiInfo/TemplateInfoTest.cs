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
            string path = Path.GetTempFileName();
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Surveillance.xml", path);
                TemplateInfo templateInfo;
                Assert.IsTrue(TemplateInfo.TryRead(path, out templateInfo));
                Assert.AreEqual("Surveillance", templateInfo.Name);
                Assert.AreEqual("Description for Surveillance template", templateInfo.Description);
                Assert.AreEqual(TemplateLevel.View, templateInfo.Level);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
