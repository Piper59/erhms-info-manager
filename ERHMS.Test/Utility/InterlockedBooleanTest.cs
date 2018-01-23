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
            for (int index = 0; index < 10; index++)
            {
                Thread thread = new Thread(() =>
                {
                    threadCount++;
                    if (flag.Exchange(true) == false)
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
    }
}
