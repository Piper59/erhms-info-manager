using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class ProcessExtensionsTest
    {
        [Test]
        public void FormatArgsTest()
        {
            Assert.AreEqual("\"one\"", FormatArgs("one"));
            Assert.AreEqual("\"one\" \"two\"", FormatArgs("one", "two"));
            Assert.Catch(() =>
            {
                FormatArgs("one", null, "three");
            });
            Assert.AreEqual("\"one\" \"\" \"three\"", FormatArgs("one", "", "three"));
            Assert.AreEqual("\"one\" \"two \"\"three\"\" four\" \"five\"", FormatArgs("one", "two \"three\" four", "five"));
        }

        private string FormatArgs(params string[] args)
        {
            return ProcessExtensions.FormatArgs(args);
        }
    }
}
