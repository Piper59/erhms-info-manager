using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class DbExtensionsTest
    {
        [Test]
        public void EscapeTest()
        {
            Assert.AreEqual("[Hello, world!]", DbExtensions.Escape("Hello, world!"));
            Assert.AreEqual("[Hello, [world]]!]", DbExtensions.Escape("Hello, [world]!"));
        }
    }
}
