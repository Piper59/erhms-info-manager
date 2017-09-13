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

        [Test]
        public void SerializeAndDeserializeTest()
        {
            Assert.IsNull(DynamicExtensions.Deserialize(DynamicExtensions.Serialize(null)));
            SerializeAndDeserializeTest(new Person
            {
                Name = Name,
                Age = Age,
                Male = Male
            });
            SerializeAndDeserializeTest(new
            {
                Name = Name,
                Age = Age,
                Male = Male
            });
            dynamic person = new ExpandoObject();
            person.Name = Name;
            person.Age = Age;
            person.Male = Male;
            SerializeAndDeserializeTest(person);
        }

        private void SerializeAndDeserializeTest(object original)
        {
            dynamic converted = DynamicExtensions.Deserialize(DynamicExtensions.Serialize(original));
            Assert.AreEqual(converted.Name, Name);
            Assert.AreEqual(converted.Age, Age);
            Assert.AreEqual(converted.Male, Male);
        }
    }
}
