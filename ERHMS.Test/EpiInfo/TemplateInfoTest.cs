using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.Reflection;

namespace ERHMS.Test.EpiInfo
{
    public class TemplateInfoTest
    {
        [Test]
        public void TryReadTest()
        {
            using (TempFile file = new TempFile())
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.ADDFull.View.xml", file.FullName);
                TemplateInfo templateInfo;
                Assert.IsTrue(TemplateInfo.TryRead(file.FullName, out templateInfo));
                Assert.AreEqual("ADDFull", templateInfo.Name);
                Assert.AreEqual("Description for ADDFull template", templateInfo.Description);
                Assert.AreEqual(TemplateLevel.View, templateInfo.Level);
            }
        }
    }
}
