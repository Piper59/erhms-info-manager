using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.EpiInfo
{
    public class TemplateTest
    {
        [Test]
        public void TryReadTest()
        {
            FileInfo file = new FileInfo(Path.GetTempFileName());
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.EpiInfo.Surveillance.xml", file);
                Template template;
                Assert.IsTrue(Template.TryRead(file, out template));
                Assert.AreEqual("Surveillance", template.Name);
                Assert.AreEqual("Description for Surveillance.xml", template.Description);
                Assert.AreEqual(TemplateLevel.View, template.Level);
            }
            finally
            {
                file.Delete();
            }
        }
    }
}
