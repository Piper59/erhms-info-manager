using Dapper;
using ERHMS.Dapper;
using NUnit.Framework;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Test.Dapper
{
    public abstract class DatabaseBaseTestBase
    {
        protected IDatabase database;

        protected void PostSetUp()
        {
            using (IDbConnection connection = database.GetConnection())
            {
                connection.Execute("CREATE TABLE Test (Id NVARCHAR(255) NOT NULL PRIMARY KEY)");
            }
        }

        [Test]
        public void BeginTransactionTest()
        {
            using (Transaction transaction = database.BeginTransaction())
            {
                Assert.Catch(() =>
                {
                    using (database.BeginTransaction()) { }
                });
            }
        }

        private int Count(IDbConnection connection, IDbTransaction transaction)
        {
            string sql = "SELECT COUNT(*) FROM Test";
            return connection.ExecuteScalar<int>(sql, transaction: transaction);
        }

        private int Count()
        {
            using (IDbConnection connection = database.GetConnection())
            {
                return Count(connection, null);
            }
        }

        private void Insert(IDbConnection connection, string id, IDbTransaction transaction)
        {
            string sql = "INSERT INTO Test (Id) VALUES (@Id)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            connection.Execute(sql, parameters, transaction);
        }

        [Test]
        public void InvokeTest()
        {
            int count = Count();
            database.Invoke((connection, transaction) =>
            {
                Assert.IsNull(transaction);
                Insert(connection, "Invoke1", transaction);
                count++;
                Assert.AreEqual(count, Count(connection, transaction));
            });
            Assert.AreEqual(count, Count());
            using (Transaction transaction = database.BeginTransaction())
            {
                database.Invoke((connection, transactionBase) =>
                {
                    Assert.AreEqual(transaction.Base, transactionBase);
                    Insert(connection, "Invoke2", transactionBase);
                    count++;
                    Assert.AreEqual(count, Count(connection, transactionBase));
                });
                transaction.Commit();
            }
            Assert.AreEqual(count, Count());
        }

        [Test]
        public void TransactTest()
        {
            int count = Count();
            database.Transact((connection, transaction) =>
            {
                Assert.IsNotNull(transaction);
                Insert(connection, "Transact1", transaction);
                count++;
                Assert.AreEqual(count, Count(connection, transaction));
            });
            Assert.AreEqual(count, Count());
            using (Transaction transaction = database.BeginTransaction())
            {
                database.Transact((connection, transactionBase) =>
                {
                    Assert.AreEqual(transaction.Base, transactionBase);
                    Insert(connection, "Transact2", transactionBase);
                    count++;
                    Assert.AreEqual(count, Count(connection, transactionBase));
                });
                transaction.Commit();
            }
            Assert.AreEqual(count, Count());
        }
    }

    public class AccessDatabaseTest : DatabaseBaseTestBase
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OleDbConnectionStringBuilder builder;
            EmptyDatabaseTestBase.Access.SetUp(nameof(AccessDatabaseTest), out directory, out builder);
            database = new AccessDatabase(builder);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directory.Dispose();
        }
    }

    public class SqlServerDatabaseTest : DatabaseBaseTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlConnectionStringBuilder builder;
            EmptyDatabaseTestBase.SqlServer.SetUp(out builder);
            database = new SqlServerDatabase(builder);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            EmptyDatabaseTestBase.SqlServer.TearDown();
        }
    }
}
