using Dapper;
using ERHMS.Dapper;
using NUnit.Framework;
using System;
using System.Data;

namespace ERHMS.Test.Dapper
{
    public abstract class DatabaseTest
    {
        private IDatabaseCreator creator;
        private IDatabase database;

        protected abstract IDatabaseCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            creator = GetCreator();
            database = creator.GetDatabase();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        [Order(1)]
        public void ExistsAndCreateTest()
        {
            Assert.IsFalse(database.Exists());
            database.Create();
            Assert.IsTrue(database.Exists());
        }

        [Test]
        [Order(2)]
        public void TableExistsTest()
        {
            Assert.IsFalse(database.TableExists("Test"));
            using (IDbConnection connection = creator.GetConnection())
            {
                connection.Execute("CREATE TABLE [Test] ([Id] NVARCHAR(255) NOT NULL PRIMARY KEY)");
            }
            Assert.IsTrue(database.TableExists("Test"));
        }

        private int Count(IDbConnection connection, IDbTransaction transaction)
        {
            string sql = "SELECT COUNT(*) FROM [Test]";
            return connection.ExecuteScalar<int>(sql, transaction: transaction);
        }

        private int Count()
        {
            using (IDbConnection connection = creator.GetConnection())
            {
                return Count(connection, null);
            }
        }

        private void Insert(IDbConnection connection, IDbTransaction transaction)
        {
            string sql = "INSERT INTO [Test] ([Id]) VALUES (@Id)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid().ToString());
            connection.Execute(sql, parameters, transaction);
        }

        [Test]
        public void InvokeTest()
        {
            int count = Count();
            database.Invoke((connectionOuter, transactionOuter) =>
            {
                Assert.IsNull(transactionOuter);
                Insert(connectionOuter, transactionOuter);
                count++;
                Assert.AreEqual(count, Count(connectionOuter, transactionOuter));
                database.Invoke((connectionInner, transactionInner) =>
                {
                    Assert.AreEqual(connectionOuter, connectionInner);
                    Assert.IsNull(transactionInner);
                    Insert(connectionInner, transactionInner);
                    count++;
                    Assert.AreEqual(count, Count(connectionInner, transactionInner));
                });
                database.Transact((connectionInner, transactionInner) =>
                {
                    Assert.AreEqual(connectionOuter, connectionInner);
                    Assert.IsNotNull(transactionInner);
                    Insert(connectionInner, transactionInner);
                    count++;
                    Assert.AreEqual(count, Count(connectionInner, transactionInner));
                });
            });
            Assert.AreEqual(count, Count());
        }

        [Test]
        public void TransactTest()
        {
            int count = Count();
            database.Transact((connectionOuter, transactionOuter) =>
            {
                Assert.IsNotNull(transactionOuter);
                Insert(connectionOuter, transactionOuter);
                count++;
                Assert.AreEqual(count, Count(connectionOuter, transactionOuter));
                database.Invoke((connectionInner, transactionInner) =>
                {
                    Assert.AreEqual(connectionOuter, connectionInner);
                    Assert.AreEqual(transactionOuter, transactionInner);
                    Insert(connectionInner, transactionInner);
                    count++;
                    Assert.AreEqual(count, Count(connectionInner, transactionInner));
                });
                database.Transact((connectionInner, transactionInner) =>
                {
                    Assert.AreEqual(connectionOuter, connectionInner);
                    Assert.AreEqual(transactionOuter, transactionInner);
                    Insert(connectionInner, transactionInner);
                    count++;
                    Assert.AreEqual(count, Count(connectionInner, transactionInner));
                });
            });
            Assert.AreEqual(count, Count());
        }
    }

    public class AccessDatabaseTest : DatabaseTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new AccessDatabaseCreator(nameof(AccessDatabaseTest));
        }
    }

    public class SqlServerDatabaseTest : DatabaseTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new SqlServerDatabaseCreator();
        }
    }
}
