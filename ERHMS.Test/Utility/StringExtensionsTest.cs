using ERHMS.Utility;
using NUnit.Framework;
using System;
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
        public void SplitLinesTest()
        {
            string format = "Line 1{0}Line 2{0}{0}Line 3{0}{0}{0}Line 4";
            string windows = string.Format(format, "\r\n");
            string unix = string.Format(format, "\n");
            string mac = string.Format(format, "\r");
            string mixed = "Line 1\r\nLine 2\r\n\nLine 3\r\n\n\rLine 4";
            ICollection<string> expected = new string[]
            {
                "Line 1",
                "Line 2",
                "",
                "Line 3",
                "",
                "",
                "Line 4"
            };
            CollectionAssert.AreEqual(expected, windows.SplitLines());
            CollectionAssert.AreEqual(expected, unix.SplitLines());
            CollectionAssert.AreEqual(expected, mac.SplitLines());
            CollectionAssert.AreEqual(expected, mixed.SplitLines());
        }

        [Test]
        public void NormalizeNewLinesTest()
        {
            string format = "Line 1{0}Line 2{0}{0}Line 3{0}{0}{0}Line 4";
            string windows = string.Format(format, "\r\n");
            string unix = string.Format(format, "\n");
            string mac = string.Format(format, "\r");
            string mixed = "Line 1\r\nLine 2\r\n\nLine 3\r\n\n\rLine 4";
            string normalized = string.Format(format, Environment.NewLine);
            Assert.AreEqual(normalized, windows.NormalizeNewLines());
            Assert.AreEqual(normalized, unix.NormalizeNewLines());
            Assert.AreEqual(normalized, mac.NormalizeNewLines());
            Assert.AreEqual(normalized, mixed.NormalizeNewLines());
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
