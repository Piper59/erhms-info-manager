using Dapper;
using ERHMS.Dapper;
using ERHMS.Test.Utility;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data;

namespace ERHMS.Test.Dapper
{
    public class LoggingConnectionTest
    {
        private IDatabaseCreator creator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            creator = new AccessDatabaseCreator(nameof(LoggingConnectionTest));
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Log.LevelName = "DEBUG";
            creator.TearDown();
        }

        [Test]
        public void ExecuteTest()
        {
            using (IDbConnection connection = new LoggingConnection(creator.GetConnection()))
            {
                int count = LogTest.GetLineCount();
                connection.Execute("CREATE TABLE [Test] ([Id] INTEGER NOT NULL PRIMARY KEY)");
                count++;
                Assert.AreEqual(count, LogTest.GetLineCount());
                connection.Execute("INSERT INTO [Test] ([Id]) VALUES (1)");
                count++;
                Assert.AreEqual(count, LogTest.GetLineCount());
                Log.LevelName = "WARN";
                connection.Query("SELECT * FROM [Test]");
                Assert.AreEqual(count, LogTest.GetLineCount());
                Log.LevelName = "DEBUG";
                connection.Execute("DROP TABLE [Test]");
                count++;
                Assert.AreEqual(count, LogTest.GetLineCount());
            }
        }
    }
}
