using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class AttributeExtensionsTest
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class DummyAttribute : Attribute
        {
            public int Value { get; private set; }

            public DummyAttribute(int value)
            {
                Value = value;
            }
        }

        private class Dummy0 { }

        [Dummy(1)]
        private class Dummy1 { }

        [Dummy(1)]
        [Dummy(2)]
        [Dummy(3)]
        private class Dummy3 { }

        [Test]
        public void GetCustomAttributesTest()
        {
            Assert.AreEqual(0, typeof(Dummy0).GetCustomAttributes<DummyAttribute>().Count());
            Assert.AreEqual(1, typeof(Dummy1).GetCustomAttributes<DummyAttribute>().Count());
            Assert.AreEqual(3, typeof(Dummy3).GetCustomAttributes<DummyAttribute>().Count());
        }

        [Test]
        public void GetCustomAttributeTest()
        {
            Assert.IsNull(typeof(Dummy0).GetCustomAttribute<DummyAttribute>());
            Assert.AreEqual(1, typeof(Dummy1).GetCustomAttribute<DummyAttribute>().Value);
            Assert.Catch(() =>
            {
                typeof(Dummy3).GetCustomAttribute<DummyAttribute>();
            });
        }

        [Test]
        public void HasCustomAttributeTest()
        {
            Assert.IsFalse(typeof(Dummy0).HasCustomAttribute<DummyAttribute>());
            Assert.IsTrue(typeof(Dummy1).HasCustomAttribute<DummyAttribute>());
            Assert.IsTrue(typeof(Dummy3).HasCustomAttribute<DummyAttribute>());
        }
    }
}
