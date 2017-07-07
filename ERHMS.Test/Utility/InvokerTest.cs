using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class InvokerTest
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
            Assert.AreEqual(0, counter.Value);
            new Invoker
            {
                Object = counter,
                MethodName = "Increment"
            }.Invoke();
            Assert.AreEqual(1, counter.Value);
            new Invoker
            {
                Object = counter,
                MethodName = "Increment",
                ArgTypes = new Type[] { typeof(int) }
            }.Invoke(2);
            Assert.AreEqual(3, counter.Value);
            new Invoker
            {
                Object = counter,
                DeclaringType = typeof(Number),
                MethodName = "Reset"
            }.Invoke();
            Assert.AreEqual(0, counter.Value);
            Assert.AreEqual(1, new Invoker
            {
                DeclaringType = typeof(Counter),
                MethodName = "GetStep"
            }.Invoke());
        }
    }
}
