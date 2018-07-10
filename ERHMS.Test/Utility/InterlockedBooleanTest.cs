using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace ERHMS.Test.Utility
{
    public class InterlockedBooleanTest
    {
        [Test]
        public void ExchangeTest()
        {
            InterlockedBoolean flag = new InterlockedBoolean(false);
            ICollection<Thread> threads = new List<Thread>();
            int threadCount = 0;
            int exchangeCount = 0;
            for (int index = 0; index < 100; index++)
            {
                Thread thread = new Thread(() =>
                {
                    threadCount++;
                    if (!flag.Exchange(true))
                    {
                        exchangeCount++;
                    }
                });
                thread.Start();
                threads.Add(thread);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            Assert.IsTrue(flag.Value);
            Assert.AreEqual(threads.Count, threadCount);
            Assert.AreEqual(1, exchangeCount);
        }

        [Test]
        public void CompareExchangeTest()
        {
            InterlockedBoolean flag = new InterlockedBoolean(false);
            Assert.IsFalse(flag.CompareExchange(true, true));
            Assert.IsFalse(flag.Value);
            Assert.IsFalse(flag.CompareExchange(true, false));
            Assert.IsTrue(flag.Value);
            Assert.IsTrue(flag.CompareExchange(false, false));
            Assert.IsTrue(flag.Value);
            Assert.IsTrue(flag.CompareExchange(false, true));
            Assert.IsFalse(flag.Value);
        }
    }
}
