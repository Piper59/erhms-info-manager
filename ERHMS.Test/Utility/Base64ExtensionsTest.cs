using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class Base64ExtensionsTest
    {
        [Serializable]
        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Male { get; set; }
        }

        [Test]
        public void ToAndFromBase64StringTest()
        {
            Assert.AreEqual("", Base64Extensions.ToBase64String(null));
            Assert.IsNull(Base64Extensions.FromBase64String(""));
            Person original = new Person
            {
                Name = "John Doe",
                Age = 20,
                Male = true
            };
            Person converted = (Person)Base64Extensions.FromBase64String(Base64Extensions.ToBase64String(original));
            Assert.AreEqual(original.Name, converted.Name);
            Assert.AreEqual(original.Age, converted.Age);
            Assert.AreEqual(original.Male, converted.Male);
        }
    }
}
