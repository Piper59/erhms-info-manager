using Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data.SqlClient;

namespace ERHMS.Test.Utility
{
    public class SqlClientExtensionsTest
    {
        private SqlConnectionStringBuilder builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            builder = Config.GetTestConnectionStringBuilder();
            using (SqlConnection connection = Config.GetMasterConnection())
            {
                string sql = string.Format("CREATE DATABASE {0}", DbExtensions.Escape(builder.InitialCatalog));
                connection.Execute(sql);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            using (SqlConnection connection = Config.GetMasterConnection())
            {
                string sql = string.Format("DROP DATABASE {0}", DbExtensions.Escape(builder.InitialCatalog));
                connection.Execute(sql);
            }
        }

        [Test]
        public void GetMasterConnectionTest()
        {
            using (SqlConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM [sys].[databases] WHERE [name] = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", builder.InitialCatalog);
                Assert.AreEqual(1, connection.ExecuteScalar<int>(sql, parameters));
            }
        }

        [Test]
        public void ExecuteNonQueryTest()
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                connection.ExecuteNonQuery("CREATE TABLE {0} ({1} INTEGER NOT NULL IDENTITY PRIMARY KEY)", "Table", "Column");
                string sql = "SELECT COUNT(*) FROM [Table]";
                Assert.AreEqual(0, connection.ExecuteScalar<int>(sql));
                connection.ExecuteNonQuery("DROP TABLE {0}", "Table");
            }
        }
    }
}
