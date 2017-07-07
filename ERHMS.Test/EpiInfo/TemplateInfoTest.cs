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
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.ADDFull.View.xml", path);
                TemplateInfo templateInfo;
                Assert.IsTrue(TemplateInfo.TryRead(path, out templateInfo));
                Assert.AreEqual("ADDFull", templateInfo.Name);
                Assert.AreEqual("Description for ADDFull template", templateInfo.Description);
                Assert.AreEqual(TemplateLevel.View, templateInfo.Level);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
