using Dapper;
using ERHMS.Dapper;
using ERHMS.Test.Utility;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

namespace ERHMS.Test.Dapper
{
    public class LoggingConnectionTest
    {
        [Test]
        public void ExecuteTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(ExecuteTest)))
            {
                string path = directory.CombinePaths(nameof(ExecuteTest) + ".mdb");
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", path);
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
                {
                    Provider = OleDbExtensions.Providers.Access,
                    DataSource = path
                };
                using (IDbConnection connection = new LoggingConnection(new OleDbConnection(builder.ConnectionString)))
                {
                    int count = LogTest.GetLineCount();
                    connection.Execute("CREATE TABLE Test (Id INTEGER NOT NULL PRIMARY KEY)");
                    count++;
                    Assert.AreEqual(count, LogTest.GetLineCount());
                    connection.Execute("INSERT INTO Test (Id) VALUES (1)");
                    count++;
                    Assert.AreEqual(count, LogTest.GetLineCount());
                    connection.Query("SELECT * FROM Test");
                    count++;
                    Assert.AreEqual(count, LogTest.GetLineCount());
                    connection.Execute("DROP TABLE Test");
                    count++;
                    Assert.AreEqual(count, LogTest.GetLineCount());
                }
            }
        }
    }
}
