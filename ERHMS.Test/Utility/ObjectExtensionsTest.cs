using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class ObjectExtensionsTest
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public Person() { }

            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public override int GetHashCode()
            {
                return ObjectExtensions.GetHashCode(FirstName, LastName);
            }
        }

        [Test]
        public void GetHashCodeTest()
        {
            Person person1 = new Person();
            Person person2 = new Person();
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            person1 = new Person("John", "Doe");
            person2 = new Person("John", "Doe");
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            person2.FirstName = "Jane";
            Assert.AreNotEqual(person1.GetHashCode(), person2.GetHashCode());
        }
    }
}
