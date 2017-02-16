using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using NUnit.Framework;
using System;
using System.IO;

namespace ERHMS.Test.EpiInfo
{
    using Test = Test.Wrappers.Test;

    public class WrapperTest
    {
        private Wrapper wrapper;

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

        [TearDown]
        public void TearDown()
        {
            if (wrapper != null)
            {
                if (!wrapper.Exited.WaitOne(10000))
                {
                    Assert.Fail("Wrapper is not responding.");
                }
                wrapper = null;
            }
        }

        [Test]
        public void OutTest()
        {
            wrapper = Test.OutTest();
            wrapper.Invoke();
            Assert.AreEqual("Hello, world!", wrapper.ReadLine());
            Assert.IsNull(wrapper.ReadLine());
        }

        [Test]
        public void InAndOutTest()
        {
            wrapper = Test.InAndOutTest();
            wrapper.Invoke();
            Random random = new Random();
            try
            {
                for (int index = 0; index < 10; index++)
                {
                    int value = random.Next();
                    wrapper.WriteLine(value);
                    Assert.AreEqual(value.ToString(), wrapper.ReadLine());
                }
            }
            finally
            {
                wrapper.Close();
            }
        }

        [Test]
        public void ArgsTest1()
        {
            wrapper = Test.ArgsTest("johnd", "John Doe", 20.25, true);
            wrapper.Invoke();
            Assert.AreEqual("ID = johnd", wrapper.ReadLine());
            Assert.AreEqual("Name = John Doe", wrapper.ReadLine());
            Assert.AreEqual("Age = 20 years 3 months", wrapper.ReadLine());
            Assert.AreEqual("Gender = M", wrapper.ReadLine());
        }

        [Test]
        public void ArgsTest2()
        {
            wrapper = Test.ArgsTest(null, "Jane Doe", 30.75, false);
            wrapper.Invoke();
            Assert.AreEqual("ID = N/A", wrapper.ReadLine());
            Assert.AreEqual("Name = Jane Doe", wrapper.ReadLine());
            Assert.AreEqual("Age = 30 years 9 months", wrapper.ReadLine());
            Assert.AreEqual("Gender = F", wrapper.ReadLine());
        }

        [Test]
        public void LongArgTest()
        {
            wrapper = Test.LongArgTest(new string('A', 10000));
            wrapper.Invoke();
            Assert.AreEqual("10000", wrapper.ReadLine());
        }

        [Test]
        public void EventTypeTest()
        {
            wrapper = Test.EventTypeTest();
            bool raised = false;
            wrapper.Event += (sender, e) =>
            {
                raised = true;
                Assert.AreEqual(WrapperEventType.Default, e.Type);
            };
            wrapper.Invoke();
            wrapper.Exited.WaitOne();
            wrapper = null;
            Assert.IsTrue(raised);
        }

        [Test]
        public void EventPropertiesTest()
        {
            wrapper = Test.EventPropertiesTest();
            bool raised = false;
            wrapper.Event += (sender, e) =>
            {
                raised = true;
                Assert.AreEqual("John Doe", e.Properties.Name);
                Assert.AreEqual(20, e.Properties.Age);
                Assert.AreEqual(true, e.Properties.Male);
            };
            wrapper.Invoke();
            wrapper.Exited.WaitOne();
            wrapper = null;
            Assert.IsTrue(raised);
        }
    }
}
