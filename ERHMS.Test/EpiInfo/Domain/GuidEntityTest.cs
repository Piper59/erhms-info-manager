using Dapper;
using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.Domain
{
    public class GuidEntityTest
    {
        private class Person : GuidEntity
        {
            protected override string Guid
            {
                get { return PersonId; }
                set { PersonId = value; }
            }

            public string PersonId
            {
                get { return GetProperty<string>(nameof(PersonId)); }
                set { SetProperty(nameof(PersonId), value); }
            }

            public string GenderId
            {
                get { return GetProperty<string>(nameof(GenderId)); }
                set { SetProperty(nameof(GenderId), value); }
            }

            public string Name
            {
                get { return GetProperty<string>(nameof(Name)); }
                set { SetProperty(nameof(Name), value); }
            }

            public DateTime? BirthDate
            {
                get { return GetProperty<DateTime?>(nameof(BirthDate)); }
                set { SetProperty(nameof(BirthDate), value?.RemoveMilliseconds()); }
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

            public Person(bool @new)
                : base(@new)
            {
                AddSynonym(nameof(PersonId), "Guid");
            }

            public Person()
                : this(false) { }
        }

        private IDatabaseCreator creator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            creator = new SqlServerDatabaseCreator();
            creator.SetUp();
            using (IDbConnection connection = creator.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql")));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        public void PropertyChangedTest()
        {
            Person person = new Person(true);
            ICollection<string> actual = new List<string>();
            ICollection<string> expected = new List<string>();
            person.PropertyChanged += (sender, e) =>
            {
                actual.Add(e.PropertyName);
            };
            {
                person.Name = "Doe";
                expected.Add(nameof(Person.Name));
                person.BirthDate = new DateTime(1980, 1, 1);
                expected.Add(nameof(Person.BirthDate));
                person.Height = null;
                expected.Add(nameof(Person.Height));
                person.Weight = null;
                expected.Add(nameof(Person.Weight));
                CollectionAssert.AreEqual(expected, actual);
            }
            actual.Clear();
            expected.Clear();
            {
                person.Name = "Doe";
                person.BirthDate = new DateTime(1990, 1, 1);
                expected.Add(nameof(Person.BirthDate));
                person.Height = 6.0;
                expected.Add(nameof(Person.Height));
                person.Weight = null;
                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void GetAndSetPropertyTest()
        {
            Person person = new Person(true);
            string name = "Doe";
            Assert.IsTrue(person.SetProperty(nameof(Person.Name), name));
            Assert.IsFalse(person.SetProperty(nameof(Person.Name), name));
            Assert.AreEqual(name, person.GetProperty(nameof(Person.Name)));
            Assert.DoesNotThrow(() =>
            {
                person.GetProperty(nameof(Person.BirthDate));
            });
        }

        [Test]
        public void TryGetPropertyTest()
        {
            Person person = new Person(true);
            string actual;
            Assert.IsFalse(person.TryGetProperty(nameof(Person.Name), out actual));
            Assert.IsNull(actual);
            string expected = "Doe";
            person.SetProperty(nameof(Person.Name), expected);
            Assert.IsTrue(person.TryGetProperty(nameof(Person.Name), out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PropertyEqualsTest()
        {
            Person person = new Person(true);
            Assert.IsFalse(person.PropertyEquals(nameof(Person.Name), null));
            person.SetProperty(nameof(Person.Name), null);
            Assert.IsTrue(person.PropertyEquals(nameof(Person.Name), null));
            string name = "Doe";
            person.SetProperty(nameof(Person.Name), name);
            Assert.IsTrue(person.PropertyEquals(nameof(Person.Name), name));
        }

        [Test]
        public void SetPropertiesTest()
        {
            using (IDbConnection connection = creator.GetConnection())
            {
                string sql = @"
                    SELECT [PersonId], [Name], [BirthDate], [Height] AS [P.Height], [Weight] AS [P.Weight]
                    FROM [Person]
                    WHERE [PersonId] = @PersonId";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PersonId", "999181b4-8445-e585-5178-74a9e11e75fa");
                using (IDataReader reader = connection.ExecuteReader(sql, parameters))
                {
                    StringAssert.Contains(".", reader.GetName(3));
                    StringAssert.Contains(".", reader.GetName(4));
                    while (reader.Read())
                    {
                        Person person = new Person(false);
                        person.SetProperties(reader);
                        Assert.AreEqual(parameters.Get<string>("@PersonId"), person.PersonId);
                        Assert.AreEqual("Graham", person.Name);
                        Assert.AreEqual(new DateTime(1986, 9, 14), person.BirthDate);
                        Assert.AreEqual(5.8, person.Height);
                        Assert.AreEqual(180.5, person.Weight);
                    }
                }
            }
        }

        [Test]
        public void AddSynonymTest()
        {
            Person person = new Person(true);
            ICollection<string> actual = new List<string>();
            ICollection<string> expected = new List<string>();
            person.PropertyChanged += (sender, e) =>
            {
                actual.Add(e.PropertyName);
            };
            person.PersonId = Guid.NewGuid().ToString();
            expected.Add(nameof(Person.PersonId));
            expected.Add("Guid");
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void TryGetAndSetMemberTest()
        {
            dynamic person = new Person(true);
            Assert.IsNull(person.Name);
            string name = "Doe";
            person.Name = name;
            Assert.AreEqual(name, person.Name);
        }

        [Test]
        public void CloneTest()
        {
            dynamic original = new Person(true);
            original.Value = 1;
            original.Object = new ExpandoObject();
            original.Object.Value = 1;
            original.Entity = new Person(true);
            original.Entity.Value = 1;
            dynamic clone = original.Clone();
            Assert.AreEqual(1, clone.Value);
            Assert.AreEqual(1, clone.Object.Value);
            Assert.AreEqual(1, clone.Entity.Value);
            original.Value = 2;
            original.Object.Value = 2;
            original.Entity.Value = 2;
            Assert.AreEqual(1, clone.Value);
            Assert.AreEqual(2, clone.Object.Value);
            Assert.AreEqual(1, clone.Entity.Value);
        }

        [Test]
        public void GetHashCodeAndEqualsTest()
        {
            Person person1 = new Person(false)
            {
                PersonId = "999181b4-8445-e585-5178-74a9e11e75fa"
            };
            Person person2;
            person2 = null;
            GetHashCodeAndEqualsTest(false, person1, person2);
            person2 = new Person(false);
            GetHashCodeAndEqualsTest(false, person1, person2);
            person2 = new Person(false)
            {
                PersonId = person1.PersonId.ToLower()
            };
            GetHashCodeAndEqualsTest(true, person1, person2);
            person2 = new Person(false)
            {
                PersonId = person1.PersonId.ToUpper()
            };
            GetHashCodeAndEqualsTest(true, person1, person2);
            person2 = new Person(false)
            {
                PersonId = "c15c66cf-b6c9-08a4-1c24-552105cac021"
            };
            GetHashCodeAndEqualsTest(false, person1, person2);
        }

        private void GetHashCodeAndEqualsTest(bool expected, Person person1, Person person2)
        {
            Assert.AreEqual(expected, person1?.GetHashCode() == person2?.GetHashCode());
            Assert.AreEqual(expected, Equals(person1, person2));
            if (expected)
            {
                Assert.IsFalse(ReferenceEquals(person1, person2));
            }
        }
    }
}
