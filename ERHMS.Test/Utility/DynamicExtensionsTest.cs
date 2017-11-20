using ERHMS.Utility;
using NUnit.Framework;
using System.Dynamic;

namespace ERHMS.Test.Utility
{
    public class DynamicExtensionsTest
    {
        private const string Name = "John Doe";
        private const int Age = 20;
        private const bool Male = true;

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Male { get; set; }
        }

        private object Initialize(dynamic person)
        {
            person.Name = Name;
            person.Age = Age;
            person.Male = Male;
            return person;
        }

        private object Initialize()
        {
            return new
            {
                Name = Name,
                Age = Age,
                Male = Male
            };
        }

        [Test]
        public void SerializeAndDeserializeTest()
        {
            Assert.IsNull(DynamicExtensions.Deserialize(DynamicExtensions.Serialize(null)));
            SerializeAndDeserializeTest(Initialize(new Person()));
            SerializeAndDeserializeTest(Initialize());
            SerializeAndDeserializeTest(Initialize(new ExpandoObject()));
        }

        private void SerializeAndDeserializeTest(object original)
        {
            dynamic converted = DynamicExtensions.Deserialize(DynamicExtensions.Serialize(original));
            Assert.AreEqual(Name, converted.Name);
            Assert.AreEqual(Age, converted.Age);
            Assert.AreEqual(Male, converted.Male);
        }
    }
}
