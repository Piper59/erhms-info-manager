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

            public override bool Equals(object obj)
            {
                Person person = obj as Person;
                return person != null && person.FirstName == FirstName && person.LastName == LastName;
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
            Assert.IsTrue(person1.Equals(person2));
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            person1 = new Person("John", "Doe");
            person2 = new Person("John", "Doe");
            Assert.IsTrue(person1.Equals(person2));
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            person2.FirstName = "Jane";
            Assert.IsFalse(person1.Equals(person2));
            Assert.AreNotEqual(person1.GetHashCode(), person2.GetHashCode());
        }
    }
}
