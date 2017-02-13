using Epi;
using ERHMS.EpiInfo;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace ERHMS.Test.EpiInfo
{
    public class WrapperTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ConfigurationExtensions.Create(Environment.CurrentDirectory).Save();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
        }

        [Test]
        public void OutTest()
        {
            Wrapper wrapper = TestProgram.OutTest();
            wrapper.Invoke();
            Assert.AreEqual("Hello, world!", wrapper.ReadLine());
            Assert.IsNull(wrapper.ReadLine());
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void InAndOutTest()
        {
            Wrapper wrapper = TestProgram.InAndOutTest();
            wrapper.Invoke();
            for (int value = 1; value <= 10; value++)
            {
                wrapper.WriteLine(value);
                Assert.AreEqual(value.ToString(), wrapper.ReadLine());
            }
            wrapper.Close();
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void ArgsTest()
        {
            Wrapper wrapper = TestProgram.ArgsTest(Enumerable.Range(1, 10));
            wrapper.Invoke();
            Assert.AreEqual("55", wrapper.ReadLine());
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void EventTest()
        {
            Wrapper wrapper = TestProgram.EventTest();
            bool raised = false;
            wrapper.Event += (sender, e) =>
            {
                raised = true;
                Assert.AreEqual(WrapperEventType.Default, e.Type);
                Assert.AreEqual("", e.Properties.Empty);
                Assert.AreEqual("'Hello, world!'", e.Properties.Message);
                Assert.AreEqual("1 + 2 + 3 = 6", e.Properties.Math);
                Assert.AreEqual("A & B & C = D", e.Properties.Logic);
                Assert.AreEqual("42", e.Properties.Number);
            };
            wrapper.Invoke();
            wrapper.Exited.WaitOne();
            Assert.IsTrue(raised);
        }
    }
}
