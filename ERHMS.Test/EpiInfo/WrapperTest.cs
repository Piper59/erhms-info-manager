using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using NUnit.Framework;
using System;
using System.IO;

namespace ERHMS.Test.EpiInfo
{
    using Test = Wrappers.Test;

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
            Wrapper wrapper = Test.OutTest();
            wrapper.Invoke();
            Assert.AreEqual("Hello, world!", wrapper.ReadLine());
            Assert.IsNull(wrapper.ReadLine());
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void InAndOutTest()
        {
            Wrapper wrapper = Test.InAndOutTest();
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
            {
                Wrapper wrapper = Test.ArgsTest("John", 20, true, DayOfWeek.Sunday);
                wrapper.Invoke();
                Assert.AreEqual("John is 20 years old.", wrapper.ReadLine());
                Assert.AreEqual("He will turn 21 on Sunday.", wrapper.ReadLine());
                wrapper.Exited.WaitOne();
            }
            {
                Wrapper wrapper = Test.ArgsTest("Jane", 30, false, DayOfWeek.Monday);
                wrapper.Invoke();
                Assert.AreEqual("Jane is 30 years old.", wrapper.ReadLine());
                Assert.AreEqual("She will turn 31 on Monday.", wrapper.ReadLine());
                wrapper.Exited.WaitOne();
            }
        }

        [Test]
        public void EventTest()
        {
            Wrapper wrapper = Test.EventTest();
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
