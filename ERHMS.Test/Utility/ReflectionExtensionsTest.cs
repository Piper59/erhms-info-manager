using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ReflectionExtensionsTest
    {
        private class Number
        {
            public int Value { get; protected set; }

            public Number()
            {
                Reset();
            }

            private void Reset()
            {
                Value = 0;
            }
        }

        private class Counter : Number
        {
            private void Increment()
            {
                Value += 1;
            }

            private void Increment(int step)
            {
                Value += step;
            }
        }

        [Test]
        public void InvokeTest()
        {
            Counter counter = new Counter();
            ReflectionExtensions.Invoke(counter, "Increment");
            Assert.AreEqual(1, counter.Value);
            ReflectionExtensions.Invoke(counter, "Increment", new Type[] { typeof(int) }, new object[] { 2 });
            Assert.AreEqual(3, counter.Value);
            ReflectionExtensions.Invoke(counter, typeof(Number), "Reset");
            Assert.AreEqual(0, counter.Value);
        }
    }
}
