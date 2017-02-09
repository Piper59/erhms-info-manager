using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            Assert.IsTrue("ONE TWO THREE".Split().ContainsIgnoreCase("two"));
        }

        [Test]
        public void StripTest()
        {
            Assert.AreEqual("abcdef", "123abc456def".Strip(new Regex(@"[0-9]")));
            Assert.AreEqual("abcdefghij", "a!b@c#d$e%f^g&h*i(j)".Strip(new Regex(@"[^a-z]")));
            Assert.AreEqual("abcdef", "   abcdef".Strip(new Regex(@"^[^a-z]+")));
        }

        [Test]
        public void MakeUniqueTest()
        {
            ICollection<string> values = new string[]
            {
                "test",
                "test (2)",
                "TEST (3)"
            };
            string format = "{0} ({1})";
            Assert.AreEqual("test", "test".MakeUnique(format, value => false));
            Assert.AreEqual("test (3)", "test".MakeUnique(format, value => values.Contains(value)));
            Assert.AreEqual("test (4)", "test".MakeUnique(format, value => values.ContainsIgnoreCase(value)));
        }
    }
}
