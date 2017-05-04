using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ConvertExtensionsTest
    {
        [Serializable]
        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Male { get; set; }
        }

        [Test]
        public void Base64StringTest()
        {
            Person original = new Person
            {
                Name = "John Doe",
                Age = 20,
                Male = true
            };
            string value = ConvertExtensions.ToBase64String(original);
            Person converted = (Person)ConvertExtensions.FromBase64String(value);
            Assert.AreEqual("John Doe", converted.Name);
            Assert.AreEqual(20, converted.Age);
            Assert.AreEqual(true, converted.Male);
        }

        [Test]
        public void NullableGuidTest()
        {
            Assert.IsNull(ConvertExtensions.ToNullableGuid(null));
            Assert.IsNull(ConvertExtensions.ToNullableGuid(""));
            Assert.AreEqual(Guid.Empty, ConvertExtensions.ToNullableGuid("00000000-0000-0000-0000-000000000000"));
            Assert.Catch(() =>
            {
                ConvertExtensions.ToNullableGuid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
            });
        }
    }
}
