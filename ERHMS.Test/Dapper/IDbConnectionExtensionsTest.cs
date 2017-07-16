using Dapper;
using ERHMS.Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.Dapper
{
    public abstract class IDbConnectionExtensionsTestBase
    {
        protected IDbConnection connection;
        private ICollection<Gender> genders;

        protected void PostSetUp()
        {
            connection.Open();
        }

        protected void PreTearDown()
        {
            connection.Dispose();
        }

        private int Count(string tableName, IDbTransaction transaction = null)
        {
            string sql = string.Format("SELECT COUNT(*) FROM [{0}]", tableName);
            return connection.ExecuteScalar<int>(sql, transaction: transaction);
        }

        [Test]
        [Order(1)]
        public void ExecuteTest()
        {
            Script script = new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql"));
            connection.Execute(script);
            Assert.AreEqual(1, Count("Global"));
            Assert.AreEqual(2, Count("Gender"));
            Assert.AreEqual(100, Count("Person"));
            genders = connection.Query<Gender>("SELECT * FROM Gender").ToList();
            foreach (Gender gender in genders)
            {
                Assert.AreEqual(4, gender.Pronouns.Split(';').Length);
            }
        }

        [Test]
        public void QueryTest()
        {
            string sql = @"
                SELECT P.*, NULL AS Separator, G.*
                FROM Person AS P
                INNER JOIN Gender AS G ON P.GenderId = G.GenderId
                WHERE P.Weight >= @Weight";
            Func<Person, Gender, Person> map = (person, gender) =>
            {
                person.Gender = gender;
                return person;
            };
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Weight", 200.0);
            ICollection<Person> people = connection.Query(sql, map, parameters, splitOn: "Separator").ToList();
            Assert.AreEqual(11, people.Count);
            Assert.IsTrue(people.All(person => person.GenderId == person.Gender.GenderId));
            Assert.AreEqual(2, people.Count(person => person.Gender.Name == "Female"));
        }

        [Test]
        public void SelectTest()
        {
            Assert.AreEqual(1, connection.Select<Constant>().Count());
            Assert.AreEqual(100, connection.Select<Person>().Count());
            string sql = "WHERE GenderId = @GenderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GenderId", genders.Single(gender => gender.Name == "Male").GenderId);
            Assert.AreEqual(51, connection.Select<Person>(sql, parameters).Count());
            Person person = connection.Select<Person>("ORDER BY BirthDate").First();
            Assert.AreEqual("Sims", person.Name);
            Assert.AreEqual(new DateTime(1980, 3, 2), person.BirthDate);
        }

        [Test]
        public void SelectByIdTest()
        {
            Constant constant = connection.SelectById<Constant>(1);
            Assert.AreEqual(1, constant.ConstantId);
            Assert.AreEqual("Version", constant.Name);
            Assert.AreEqual("1.0", constant.Value);
            Assert.IsNull(connection.SelectById<Constant>(2));
            Person person = connection.SelectById<Person>("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual("Graham", person.Name);
            Assert.AreEqual(new DateTime(1986, 9, 14), person.BirthDate);
            Assert.IsNull(connection.SelectById<Person>(Guid.Empty));
        }

        [Test]
        public void InsertTest()
        {
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                Constant constant = new Constant
                {
                    Name = "Message",
                    Value = "Hello, world!"
                };
                connection.Insert(constant, transaction);
                Assert.AreEqual(constant.Name, connection.SelectById<Constant>(2, transaction).Name);
            }
            Assert.AreEqual(1, Count("Global"));
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                Person person = new Person
                {
                    GenderId = genders.Single(gender => gender.Name == "Male").GenderId,
                    Name = "Doe",
                    BirthDate = DateTime.Now
                };
                connection.Insert(person, transaction);
                Assert.AreEqual(person.Name, connection.SelectById<Person>(person.PersonId, transaction).Name);
            }
            Assert.AreEqual(100, Count("Person"));
        }

        [Test]
        public void UpdateTest()
        {
            Constant constant = connection.SelectById<Constant>(1);
            Assert.AreEqual("1.0", constant.Value);
            constant.Value = "2.0";
            connection.Update(constant);
            Assert.AreEqual(constant.Value, connection.SelectById<Constant>(constant.ConstantId).Value);
            Person person = connection.SelectById<Person>("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual(180.5, person.Weight);
            person.Weight -= 10.0;
            connection.Update(person);
            Assert.AreEqual(person.Weight, connection.SelectById<Person>(person.PersonId).Weight);
        }

        [Test]
        public void DeleteTest()
        {
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                connection.Delete<Constant>(transaction: transaction);
                Assert.AreEqual(0, Count("Global", transaction));
            }
            Assert.AreEqual(1, Count("Global"));
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                string sql = "WHERE Height >= @Height";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Height", 6.0);
                connection.Delete<Person>(sql, parameters, transaction);
                Assert.AreEqual(86, Count("Person", transaction));
            }
            Assert.AreEqual(100, Count("Person"));
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                Person person = connection.SelectById<Person>("999181b4-8445-e585-5178-74a9e11e75fa", transaction);
                connection.Delete(person, transaction);
                Assert.AreEqual(99, Count("Person", transaction));
            }
            Assert.AreEqual(100, Count("Person"));
        }

        [Test]
        public void DeleteByIdTest()
        {
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                connection.DeleteById<Constant>(1, transaction);
                Assert.AreEqual(0, Count("Global", transaction));
            }
            Assert.AreEqual(1, Count("Global"));
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                connection.DeleteById<Person>("999181b4-8445-e585-5178-74a9e11e75fa", transaction);
                Assert.AreEqual(99, Count("Person", transaction));
            }
            Assert.AreEqual(100, Count("Person"));
        }
    }

    public class OleDbConnectionExtensionsTest : IDbConnectionExtensionsTestBase
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OleDbConnectionStringBuilder builder;
            DatabaseTestBase.Access.SetUp(nameof(OleDbConnectionExtensionsTest), out directory, out builder);
            connection = new OleDbConnection(builder.ConnectionString);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PreTearDown();
            directory.Dispose();
        }
    }

    public class SqlConnectionExtensionsTest : IDbConnectionExtensionsTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlConnectionStringBuilder builder;
            DatabaseTestBase.SqlServer.SetUp(out builder);
            connection = new SqlConnection(builder.ConnectionString);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PreTearDown();
            DatabaseTestBase.SqlServer.TearDown();
        }
    }
}
