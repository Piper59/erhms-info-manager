﻿using Dapper;
using ERHMS.Dapper;
using ERHMS.Test.Utility;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data;

namespace ERHMS.Test.Dapper
{
    public abstract class LoggingConnectionTest
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
                LogTest.LineCountTest(++count);
                connection.Execute("INSERT INTO [Test] ([Id]) VALUES (1)");
                LogTest.LineCountTest(++count);
                Log.LevelName = "WARN";
                connection.Query("SELECT * FROM [Test]");
                LogTest.LineCountTest(count);
                Log.LevelName = "DEBUG";
                connection.Execute("DROP TABLE [Test]");
                LogTest.LineCountTest(++count);
            }
        }
    }

    public class AccessLoggingConnectionTest : LoggingConnectionTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return AccessDatabaseCreator.ForName(nameof(AccessLoggingConnectionTest));
        }
    }

    public class SqlServerLoggingConnectionTest : LoggingConnectionTest
    {
        protected override IDatabaseCreator GetCreator()
        {
            return new SqlServerDatabaseCreator();
        }
    }
}
