using Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System.Configuration;
using System.Data.SqlClient;

namespace ERHMS.Test.Utility
{
    public class SqlClientExtensionsTest
    {
        private SqlConnectionStringBuilder builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString)
            {
                Pooling = false
            };
        }

        [Test]
        public void GetMasterConnectionTest()
        {
            using (SqlConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
            {
                connection.Open();
                connection.ExecuteNonQuery("CREATE DATABASE {0}", builder.InitialCatalog);
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", builder.InitialCatalog);
                Assert.AreEqual(1, connection.ExecuteScalar<int>(sql, parameters));
                ExecuteNonQueryTest();
                connection.ExecuteNonQuery("DROP DATABASE {0}", builder.InitialCatalog);
            }
        }

        private void ExecuteNonQueryTest()
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                connection.ExecuteNonQuery("CREATE TABLE {0} ({1} INTEGER NOT NULL IDENTITY PRIMARY KEY)", "Table", "Column");
                connection.ExecuteNonQuery("DROP TABLE {0}", "Table");
            }
        }
    }
}
