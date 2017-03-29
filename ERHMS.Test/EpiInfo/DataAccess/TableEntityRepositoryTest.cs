using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public class TableEntityRepositoryTest : SampleTestBase
    {
        private class Person : TableEntity
        {
            public override string Guid
            {
                get { return PersonId; }
                set { PersonId = value; }
            }

            public string PersonId
            {
                get { return GetProperty<string>(nameof(PersonId)); }
                set { SetProperty(nameof(PersonId), value); }
            }

            public int? GenderId
            {
                get { return GetProperty<int?>(nameof(GenderId)); }
                set { SetProperty(nameof(GenderId), value); }
            }

            public string LastName
            {
                get { return GetProperty<string>(nameof(LastName)); }
                set { SetProperty(nameof(LastName), value); }
            }

            public DateTime? BirthDate
            {
                get { return GetProperty<DateTime?>(nameof(BirthDate)); }
                set { SetProperty(nameof(BirthDate), value); }
            }

            public double? Height
            {
                get { return GetProperty<double?>(nameof(Height)); }
                set { SetProperty(nameof(Height), value); }
            }

            public double? Weight
            {
                get { return GetProperty<double?>(nameof(Weight)); }
                set { SetProperty(nameof(Weight), value); }
            }

            public Person()
            {
                AddSynonym(nameof(PersonId), nameof(Guid));
            }
        }

        private class PersonRepository : TableEntityRepository<Person>
        {
            public PersonRepository(IDataDriver driver)
                : base(driver, "Person") { }

            public IEnumerable<Person> SelectByGenderId(int genderId)
            {
                return Select("[GenderId] = {@}", genderId);
            }

            public void DeleteTaller(double height)
            {
                Delete("[Height] > {@}", height);
            }
        }

        private PersonRepository people;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            IDataDriver driver = DataDriverFactory.CreateDataDriver(project);
            driver.ExecuteScript(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Person.sql"));
            people = new PersonRepository(driver);
        }

        [Test]
        [Order(1)]
        public void SelectTest()
        {
            Assert.AreEqual(100, people.Select().Count());
            Assert.AreEqual(51, people.SelectByGenderId(1).Count());
        }

        [Test]
        [Order(2)]
        public void SelectByGuidTest()
        {
            Person person = people.SelectByGuid("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual(1, person.GenderId);
            Assert.AreEqual("Graham", person.LastName);
            Assert.AreEqual(new DateTime(1986, 9, 14), person.BirthDate);
        }

        [Test]
        [Order(3)]
        public void SaveTest()
        {
            Person person = people.Create();
            person.GenderId = 1;
            person.LastName = "Doe";
            person.BirthDate = new DateTime(1980, 1, 1);
            Assert.IsTrue(person.New);
            people.Save(person);
            Assert.IsFalse(person.New);
            Assert.AreEqual(101, people.Select().Count());
        }

        [Test]
        [Order(4)]
        public void DeleteTest()
        {
            Person person = people.SelectByGuid("999181b4-8445-e585-5178-74a9e11e75fa");
            people.Delete(person);
            Assert.AreEqual(100, people.Select().Count());
            people.DeleteTaller(6.0);
            Assert.AreEqual(92, people.Select().Count());
        }
    }
}
