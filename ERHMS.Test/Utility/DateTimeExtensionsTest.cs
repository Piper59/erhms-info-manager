using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public void RemoveMillisecondsTest()
        {
            DateTime value = new DateTime(2000, 1, 1, 0, 0, 0, 1);
            Assert.AreNotEqual(0, value.Millisecond);
            Assert.AreEqual(0, value.RemoveMilliseconds().Millisecond);
        }
    }
}
