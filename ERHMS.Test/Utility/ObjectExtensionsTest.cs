using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace ERHMS.Test.Utility
{
    public class ObjectExtensionsTest
    {
        private class Person
        {
            private static readonly ReadOnlyCollection<Func<Person, object>> Identifiers = new ReadOnlyCollection<Func<Person, object>>(new Func<Person, object>[]
            {
                @this => @this.FirstName,
                @this => @this.LastName
            });

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
                return ObjectExtensions.GetHashCode(this, Identifiers);
            }

            public override bool Equals(object obj)
            {
                return ObjectExtensions.Equals(this, obj, Identifiers);
            }
        }

        [Test]
        public void GetHashCodeAndEqualsTest()
        {
            Person person1 = null;
            Person person2 = null;
            Assert.IsTrue(Equals(person1, person2));
            person1 = new Person();
            Assert.IsFalse(person1.Equals(person2));
            person2 = new Person();
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            Assert.IsTrue(person1.Equals(person2));
            person1 = new Person("John", "Doe");
            person2 = new Person("John", "Doe");
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
            Assert.IsTrue(person1.Equals(person2));
            person2.FirstName = "Jane";
            Assert.AreNotEqual(person1.GetHashCode(), person2.GetHashCode());
            Assert.IsFalse(person1.Equals(person2));
        }
    }
}
