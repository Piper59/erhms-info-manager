using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ERHMS.Test.Utility
{
    public class StringExtensionsTest
    {
        [Test]
        public void EqualsIgnoreCaseTest()
        {
            Assert.IsTrue("test".EqualsIgnoreCase("TEST"));
            Assert.IsTrue(StringExtensions.EqualsIgnoreCase(null, null));
        }

        [Test]
        public void ContainsIgnoreCaseTest()
        {
            Assert.IsTrue("Hello, world!".ContainsIgnoreCase("WORLD"));
            Assert.IsTrue("one two three".Split().ContainsIgnoreCase("TWO"));
        }

        private string GetMultiLineString(string newLine)
        {
            return string.Format("Line 1{0}Line 2{0}{0}Line 3{0}{0}{0}Line 4", newLine);
        }

        private IEnumerable<string> GetMultiLineStrings()
        {
            yield return GetMultiLineString("\r\n");
            yield return GetMultiLineString("\n");
            yield return GetMultiLineString("\r");
            yield return "Line 1\r\nLine 2\r\n\nLine 3\r\n\n\rLine 4";
        }

        [Test]
        public void SplitLinesTest()
        {
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
            foreach (string lines in GetMultiLineStrings())
            {
                CollectionAssert.AreEqual(expected, lines.SplitLines());
            }
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
