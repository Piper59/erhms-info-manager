using Dapper;
using ERHMS.Dapper;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test.Dapper
{
    public class TypeMapTest
    {
        private TypeMap typeMap;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            typeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(Person));
        }

        [Test]
        public void GetIdTest()
        {
            Assert.AreEqual(nameof(Person.PersonId), typeMap.GetId().Property.Name);
        }

        [Test]
        public void GetInsertableTest()
        {
            ICollection<string> expected = new string[]
            {
                nameof(Person.PersonId),
                nameof(Person.GenderId),
                nameof(Person.Name),
                nameof(Person.BirthDate),
                nameof(Person.Height),
                nameof(Person.Weight)
            };
            CollectionAssert.AreEquivalent(expected, typeMap.GetInsertable().Select(propertyMap => propertyMap.Property.Name));
        }

        [Test]
        public void GetUpdatableTest()
        {
            ICollection<string> expected = new string[]
            {
                nameof(Person.GenderId),
                nameof(Person.Name),
                nameof(Person.BirthDate),
                nameof(Person.Height),
                nameof(Person.Weight)
            };
            CollectionAssert.AreEquivalent(expected, typeMap.GetUpdatable().Select(propertyMap => propertyMap.Property.Name));
        }

        [Test]
        public void GetMemberTest()
        {
            string propertyName = nameof(Person.PersonId);
            GetMemberTest(propertyName, "PersonId");
            GetMemberTest(propertyName, "Person.PersonId");
            GetMemberTest(propertyName, "P.PersonId");
        }

        private void GetMemberTest(string propertyName, string columnName)
        {
            Assert.AreEqual(propertyName, typeMap.GetMember(columnName).Property.Name);
        }
    }
}
