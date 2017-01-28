using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class StringExtensionsTest
    {
        [Test]
        public void EqualsIgnoreCaseTest()
        {
            Assert.IsTrue("string".EqualsIgnoreCase("STRING"));
            Assert.IsTrue(StringExtensions.EqualsIgnoreCase(null, null));
        }

        [Test]
        public void ContainsIgnoreCaseTest()
        {
            Assert.IsTrue("Hello, world!".ContainsIgnoreCase("WORLD"));
            Assert.IsTrue("one two three".Split().ContainsIgnoreCase("THREE"));
        }

        [Test]
        public void ToPrintableTest()
        {
            char[] characters = Enumerable.Range(0, 0xffff)
                .Select(codePoint => Convert.ToChar(codePoint))
                .ToArray();
            char[] printables = new string(characters).ToPrintable().ToCharArray();
            Assert.AreEqual(printables.Length, 95);
            Assert.AreEqual(printables.Min(), 0x20);
            Assert.AreEqual(printables.Max(), 0x7e);
        }
    }
}
