using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Test.Utility
{
    public class SingleInstanceExecuterTest
    {
        [Test]
        public void ExecuteTest()
        {
            SingleInstanceExecuter executer1 = new SingleInstanceExecuter(0);
            SingleInstanceExecuter executer2 = new SingleInstanceExecuter(0);
            ManualResetEvent executer1Executing = new ManualResetEvent(false);
            ManualResetEvent executer2TimedOut = new ManualResetEvent(false);
            int value = 0;
            executer1.Executing += (sender, e) =>
            {
                executer1Executing.Set();
                executer2TimedOut.WaitOne();
                value = 1;
            };
            executer2.Executing += (sender, e) =>
            {
                value = 2;
            };
            Task task = Task.Factory.StartNew(() =>
            {
                executer1.Execute();
            });
            executer1Executing.WaitOne();
            Assert.Throws<TimeoutException>(() =>
            {
                executer2.Execute();
            });
            executer2TimedOut.Set();
            task.Wait();
            Assert.AreEqual(1, value);
            executer2.Execute();
            Assert.AreEqual(2, value);
        }
    }
}
