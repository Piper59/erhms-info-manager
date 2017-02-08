using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class ProcessExtensionsTest
    {
        [Test]
        public void EscapeArgTest()
        {
            Assert.AreEqual("\"\"", ProcessExtensions.EscapeArg(null));
            Assert.AreEqual("\"\"", ProcessExtensions.EscapeArg(""));
            Assert.AreEqual("\"one\"", ProcessExtensions.EscapeArg("one"));
            Assert.AreEqual("\"one two\"", ProcessExtensions.EscapeArg("one two"));
            Assert.AreEqual("\"one \"\"two\"\" three\"", ProcessExtensions.EscapeArg("one \"two\" three"));
            Assert.AreEqual("\"1\"", ProcessExtensions.EscapeArg(1));
        }

        [Test]
        public void FormatArgsTest()
        {
            Assert.AreEqual("\"one\"", ProcessExtensions.FormatArgs("one"));
            Assert.AreEqual("\"one\" \"two\"", ProcessExtensions.FormatArgs("one", "two"));
            Assert.AreEqual("\"one\" \"\" \"three\"", ProcessExtensions.FormatArgs("one", null, "three"));
            Assert.AreEqual("\"one\" \"\" \"three\"", ProcessExtensions.FormatArgs("one", "", "three"));
            Assert.AreEqual("\"one\" \"two \"\"three\"\" four\" \"five\"", ProcessExtensions.FormatArgs("one", "two \"three\" four", "five"));
            Assert.AreEqual("\"1\" \"2\" \"3\"", ProcessExtensions.FormatArgs(1, 2, 3));
        }
    }
}
