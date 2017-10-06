using Dapper;
using ERHMS.Dapper;
using NUnit.Framework;
using System.Data;

namespace ERHMS.Test.Dapper
{
    public abstract class ConnectionOpenerTest
    {
        private IDatabaseCreator creator;

        protected abstract IDatabaseCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
        }

        [Test]
        public void ExecuteTest()
        {
            ExecuteTest(false);
            ExecuteTest(true);
        }

        private void ExecuteTest(bool open)
        {
            using (IDbConnection connection = creator.GetConnection())
            {
                Assert.AreEqual(ConnectionState.Closed, connection.State);
                if (open)
                {
                    connection.Open();
                }
                ConnectionState state = open ? ConnectionState.Open : ConnectionState.Closed;
                Assert.AreEqual(state, connection.State);
                using (new ConnectionOpener(connection))
                {
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                    connection.Execute("CREATE TABLE [Test] ([Id] INTEGER NOT NULL PRIMARY KEY)");
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                    connection.Execute("INSERT INTO [Test] ([Id]) VALUES (1)");
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                    connection.Query("SELECT * FROM [Test]");
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                    connection.Execute("DROP TABLE [Test]");
                    Assert.AreEqual(ConnectionState.Open, connection.State);
                }
                Assert.AreEqual(state, connection.State);
            }
        }
    }

    public class AccessConnectionOpenerTest : ConnectionOpenerTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new AccessDatabaseCreator(nameof(AccessConnectionOpenerTest));
        }
    }

    public class SqlServerConnectionOpenerTest : ConnectionOpenerTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new SqlServerDatabaseCreator();
        }
    }
}
