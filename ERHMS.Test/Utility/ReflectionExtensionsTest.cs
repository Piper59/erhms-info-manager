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
                Value = 1;
            }
        }

        private class Counter : Number
        {
            private static int GetStep()
            {
                return 1;
            }

            private void Increment()
            {
                Value += GetStep();
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
            Assert.AreEqual(2, counter.Value);
            ReflectionExtensions.Invoke(counter, "Increment", new Type[] { typeof(int) }, new object[] { 2 });
            Assert.AreEqual(4, counter.Value);
            ReflectionExtensions.Invoke(counter, typeof(Number), "Reset");
            Assert.AreEqual(1, counter.Value);
            Assert.AreEqual(1, ReflectionExtensions.Invoke(typeof(Counter), "GetStep"));
        }
    }
}
