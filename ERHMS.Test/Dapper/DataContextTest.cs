﻿using Dapper;
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
    public abstract class DataContextTestBase
    {
        protected DataContext context;

        protected void PostSetUp()
        {
            using (IDbConnection connection = context.Database.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.People.sql")));
            }
        }

        [Test]
        public void SelectTest()
        {
            Assert.AreEqual(1, context.Constants.Count());
            Assert.AreEqual(100, context.People.Count());
            string sql = "WHERE Person.Weight >= @Weight";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Weight", 200.0);
            ICollection<Person> people = context.People.Select(sql, parameters).ToList();
            Assert.AreEqual(11, people.Count);
            Assert.AreEqual(2, people.Count(person => person.Gender.Name == "Female"));
        }

        [Test]
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
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                Constant constant = new Constant
                {
                    Name = "Message",
                    Value = "Hello, world!"
                };
                context.Constants.Insert(constant);
                Assert.AreEqual(constant.Name, context.Constants.SelectById(2).Name);
            }
            Assert.AreEqual(1, context.Constants.Count());
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                Person person = new Person
                {
                    GenderId = "273c6d62-be89-48df-9e04-775125bc4f6a",
                    Name = "Doe",
                    BirthDate = DateTime.Now
                };
                context.People.Insert(person);
                Assert.AreEqual(person.Name, context.People.SelectById(person.PersonId).Name);
            }
            Assert.AreEqual(100, context.People.Count());
        }

        [Test]
        public void UpdateTest()
        {
            Constant constant = context.Constants.SelectById(1);
            Assert.AreEqual("1.0", constant.Value);
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                constant.Value = "2.0";
                context.Constants.Update(constant);
                transaction.Commit();
            }
            Assert.AreEqual(constant.Value, context.Constants.SelectById(constant.ConstantId).Value);
            Person person = context.People.SelectById("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual(180.5, person.Weight);
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                person.Weight -= 10.0;
                context.People.Update(person);
                transaction.Commit();
            }
            Assert.AreEqual(person.Weight, context.People.SelectById(person.PersonId).Weight);
        }

        [Test]
        public void DeleteTest()
        {
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                context.Constants.Delete();
                Assert.AreEqual(0, context.Constants.Count());
            }
            Assert.AreEqual(1, context.Constants.Count());
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                string sql = "WHERE Height >= @Height";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Height", 6.0);
                context.People.Delete(sql, parameters);
                Assert.AreEqual(86, context.People.Count());
            }
            Assert.AreEqual(100, context.People.Count());
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                Person person = context.People.SelectById("999181b4-8445-e585-5178-74a9e11e75fa");
                context.People.Delete(person);
                Assert.AreEqual(99, context.People.Count());
            }
            Assert.AreEqual(100, context.People.Count());
        }

        [Test]
        public void DeleteByIdTest()
        {
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                context.Constants.DeleteById(1);
                Assert.AreEqual(0, context.Constants.Count());
            }
            Assert.AreEqual(1, context.Constants.Count());
            using (IDbTransaction transaction = context.Database.BeginTransaction())
            {
                context.People.DeleteById("999181b4-8445-e585-5178-74a9e11e75fa");
                Assert.AreEqual(99, context.People.Count());
            }
            Assert.AreEqual(100, context.People.Count());
        }
    }

    public class OleDbDataContextTest : DataContextTestBase
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OleDbConnectionStringBuilder builder;
            DatabaseTestBase.Access.SetUp(nameof(OleDbDataContextTest), out directory, out builder);
            context = new DataContext(new AccessDatabase(builder));
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directory.Dispose();
        }
    }

    public class SqlDataContextTest : DataContextTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlConnectionStringBuilder builder;
            DatabaseTestBase.SqlServer.SetUp(out builder);
            context = new DataContext(new SqlServerDatabase(builder));
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DatabaseTestBase.SqlServer.TearDown();
        }
    }
}
