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

        private int Count(IDbConnection connection = null, IDbTransaction transaction = null)
        {
            string sql = "SELECT COUNT(*) FROM [Test]";
            if (connection == null)
            {
                using (connection = creator.GetConnection())
                {
                    return connection.ExecuteScalar<int>(sql);
                }
            }
            else
            {
                return connection.ExecuteScalar<int>(sql, transaction: transaction);
            }
        }

        private void CountTest(int expected, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            Assert.AreEqual(expected, Count(connection, transaction));
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
                CountTest(++count, connectionOuter, transactionOuter);
                database.Invoke((connectionInner, transactionInner) =>
                {
                    InvokeTest(connectionOuter, connectionInner, transactionInner, false, ref count);
                });
                database.Transact((connectionInner, transactionInner) =>
                {
                    InvokeTest(connectionOuter, connectionInner, transactionInner, true, ref count);
                });
            });
            Assert.AreEqual(count, Count());
        }

        private void InvokeTest(
            IDbConnection connectionOuter,
            IDbConnection connectionInner,
            IDbTransaction transaction,
            bool transacted,
            ref int count)
        {
            Assert.AreEqual(connectionOuter, connectionInner);
            Assert.AreEqual(transacted, transaction != null);
            Insert(connectionInner, transaction);
            CountTest(++count, connectionInner, transaction);
        }

        [Test]
        public void TransactTest()
        {
            int count = Count();
            database.Transact((connectionOuter, transactionOuter) =>
            {
                Assert.IsNotNull(transactionOuter);
                Insert(connectionOuter, transactionOuter);
                CountTest(++count, connectionOuter, transactionOuter);
                database.Invoke((connectionInner, transactionInner) =>
                {
                    TransactTest(connectionOuter, transactionOuter, connectionInner, transactionInner, ref count);
                });
                database.Transact((connectionInner, transactionInner) =>
                {
                    TransactTest(connectionOuter, transactionOuter, connectionInner, transactionInner, ref count);
                });
            });
            Assert.AreEqual(count, Count());
        }

        private void TransactTest(
            IDbConnection connectionOuter,
            IDbTransaction transactionOuter,
            IDbConnection connectionInner,
            IDbTransaction transactionInner,
            ref int count)
        {
            Assert.AreEqual(connectionOuter, connectionInner);
            Assert.AreEqual(transactionOuter, transactionInner);
            Insert(connectionInner, transactionInner);
            CountTest(++count, connectionInner, transactionInner);
        }
    }

    public class AccessDatabaseTest : DatabaseTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return AccessDatabaseCreator.ForName(nameof(AccessDatabaseTest));
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
