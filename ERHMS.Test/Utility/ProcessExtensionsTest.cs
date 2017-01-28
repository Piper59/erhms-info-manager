using ERHMS.Utility;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class ProcessExtensionsTest
    {
        [Test]
        public void CreateTest()
        {
            FileInfo executable = new FileInfo("ERHMS.Test.exe");
            Process process1 = ProcessExtensions.Create(executable, new string[]
            {
                "1",
                "2 3 4",
                "\"5\""
            });
            Assert.AreEqual(process1.StartInfo.Arguments, "\"1\" \"2 3 4\" \"\"\"5\"\"\"");
            Process process2 = ProcessExtensions.Create(executable, new string[] { });
            Assert.AreEqual(process2.StartInfo.Arguments, "");
            Process process3 = ProcessExtensions.Create(executable);
            Assert.AreEqual(process3.StartInfo.Arguments, "");
        }
    }
}
