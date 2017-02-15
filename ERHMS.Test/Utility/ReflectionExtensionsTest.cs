using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ReflectionExtensionsTest
    {
        private class Number
        {
            private int start = 0;

            public int Value { get; protected set; }

            public Number()
            {
                Reset();
            }

            private void Reset()
            {
                Value = start;
            }
        }

        private class Counter : Number
        {
            private int step = 1;

            private void Increment()
            {
                Value += step;
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
            Assert.AreEqual(0, ReflectionExtensions.GetValue(counter, typeof(Number), "start"));
            Assert.AreEqual(1, ReflectionExtensions.GetValue(counter, "step"));
            ReflectionExtensions.Invoke(counter, "Increment");
            Assert.AreEqual(1, counter.Value);
            ReflectionExtensions.Invoke(counter, "Increment", new Type[] { typeof(int) }, new object[] { 2 });
            Assert.AreEqual(3, counter.Value);
            ReflectionExtensions.Invoke(counter, typeof(Number), "Reset");
            Assert.AreEqual(0, counter.Value);
        }
    }
}
