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
            Increment(counter);
            Assert.AreEqual(1, counter.Value);
            Increment(counter, 2);
            Assert.AreEqual(3, counter.Value);
            Reset(counter);
            Assert.AreEqual(0, counter.Value);
            Assert.AreEqual(1, GetStep());
        }

        private int GetStep()
        {
            Invoker invoker = new Invoker
            {
                DeclaringType = typeof(Counter),
                MethodName = "GetStep"
            };
            return (int)invoker.Invoke();
        }

        private void Reset(Counter counter)
        {
            Invoker invoker = new Invoker
            {
                Object = counter,
                DeclaringType = typeof(Number),
                MethodName = "Reset"
            };
            invoker.Invoke();
        }

        private void Increment(Counter counter)
        {
            Invoker invoker = new Invoker
            {
                Object = counter,
                MethodName = "Increment"
            };
            invoker.Invoke();
        }

        private void Increment(Counter counter, int step)
        {
            Invoker invoker = new Invoker
            {
                Object = counter,
                MethodName = "Increment",
                ArgTypes = new Type[] { typeof(int) }
            };
            invoker.Invoke(step);
        }
    }
}
