using Dapper;
using ERHMS.Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.Dapper
{
    public abstract class RepositoryTest
    {
        private class DataContext
        {
            public IDatabase Database { get; private set; }
            public IRepository<Constant> Constants { get; private set; }
            public IRepository<Gender> Genders { get; private set; }
            public IRepository<Person> People { get; private set; }

            public DataContext(IDatabase database)
            {
                Database = database;
                Constants = new Repository<Constant>(database);
                Genders = new Repository<Gender>(database);
                People = new PersonRepository(database);
            }
        }

        private IDatabaseCreator creator;
        private DataContext context;

        protected abstract IDatabaseCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            creator = GetCreator();
            creator.SetUp();
            using (IDbConnection connection = creator.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql")));
            }
            context = new DataContext(creator.GetDatabase());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        [Order(1)]
        public void CountTest()
        {
            Assert.AreEqual(1, context.Constants.Count());
            Assert.AreEqual(100, context.People.Count());
        }

        [Test]
        [Order(2)]
        public void SelectTest()
        {
            string clauses = "WHERE [Person].[Weight] >= @Weight";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Weight", 200.0);
            ICollection<Person> people = context.People.Select(clauses, parameters).ToList();
            Assert.AreEqual(11, people.Count);
            Assert.AreEqual(2, people.Count(person => person.Gender.Name == "Female"));
        }

        [Test]
        [Order(3)]
        public void SelectByIdTest()
        {
            Constant constant = context.Constants.SelectById(1);
            Assert.AreEqual(1, constant.ConstantId);
            Assert.AreEqual("Version", constant.Name);
            Assert.AreEqual("1.0", constant.Value);
            Assert.IsNull(context.Constants.SelectById(2));
            Person person = context.People.SelectById("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual("Graham", person.Name);
            Assert.AreEqual("Male", person.Gender.Name);
            Assert.AreEqual(new DateTime(1986, 9, 14), person.BirthDate);
            Assert.IsNull(context.People.SelectById(Guid.Empty.ToString()));
        }

        [Test]
        public void InsertTest()
        {
            Constant constant = new Constant
            {
                Name = "Message",
                Value = "Hello, world!"
            };
            context.Constants.Insert(constant);
            Assert.AreEqual(constant.Name, context.Constants.SelectById(2).Name);
            Person person = new Person
            {
                PersonId = Guid.NewGuid().ToString(),
                GenderId = "273c6d62-be89-48df-9e04-775125bc4f6a",
                Name = "Doe",
                BirthDate = DateTime.Now
            };
            context.People.Insert(person);
            Assert.AreEqual(person.Name, context.People.SelectById(person.PersonId).Name);
        }

        [Test]
        public void UpdateTest()
        {
            Constant constant = context.Constants.SelectById(1);
            Assert.AreEqual("1.0", constant.Value);
            constant.Value = "2.0";
            context.Constants.Update(constant);
            Assert.AreEqual(constant.Value, context.Constants.SelectById(constant.ConstantId).Value);
            Person person = context.People.SelectById("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual(180.5, person.Weight);
            person.Weight -= 10.0;
            context.People.Update(person);
            Assert.AreEqual(person.Weight, context.People.SelectById(person.PersonId).Weight);
        }

        [Test]
        public void DeleteTest()
        {
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    context.Constants.Delete();
                    Assert.AreEqual(0, context.Constants.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
            Assert.AreEqual(1, context.Constants.Count());
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    string clauses = "WHERE [Height] >= @Height";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@Height", 6.0);
                    context.People.Delete(clauses, parameters);
                    Assert.AreEqual(86, context.People.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
            Assert.AreEqual(100, context.People.Count());
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    Person person = context.People.SelectById("999181b4-8445-e585-5178-74a9e11e75fa");
                    context.People.Delete(person);
                    Assert.AreEqual(99, context.People.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
            Assert.AreEqual(100, context.People.Count());
        }

        [Test]
        public void DeleteByIdTest()
        {
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    context.Constants.DeleteById(1);
                    Assert.AreEqual(0, context.Constants.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
            Assert.AreEqual(1, context.Constants.Count());
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    context.People.DeleteById("999181b4-8445-e585-5178-74a9e11e75fa");
                    Assert.AreEqual(99, context.People.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
            Assert.AreEqual(100, context.People.Count());
        }
    }

    public class AccessRepositoryTest : RepositoryTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return AccessDatabaseCreator.ForName(nameof(AccessRepositoryTest));
        }
    }

    public class SqlServerRepositoryTest : RepositoryTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new SqlServerDatabaseCreator();
        }
    }
}
