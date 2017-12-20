using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public void AreInOrderTest()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1.0);
            Assert.IsTrue(DateTimeExtensions.AreInOrder(null, null));
            Assert.IsTrue(DateTimeExtensions.AreInOrder(today, null));
            Assert.IsTrue(DateTimeExtensions.AreInOrder(null, tomorrow));
            Assert.IsTrue(DateTimeExtensions.AreInOrder(today, tomorrow));
            Assert.IsFalse(DateTimeExtensions.AreInOrder(tomorrow, today));
        }

        [Test]
        public void RemoveMillisecondsTest()
        {
            DateTime value = new DateTime(2000, 1, 1, 0, 0, 0, 1);
            Assert.AreNotEqual(0, value.Millisecond);
            Assert.AreEqual(0, value.RemoveMilliseconds().Millisecond);
        }
    }
}
