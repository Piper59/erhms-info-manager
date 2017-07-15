using Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ERHMS.Test.Utility
{
    public class SqlClientExtensionsTest
    {
        [Test]
        public void GetMasterConnectionTest()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString);
            using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
            {
                connection.Execute(string.Format("CREATE DATABASE [{0}]", builder.InitialCatalog));
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", builder.InitialCatalog);
                Assert.AreEqual(1, connection.ExecuteScalar<int>(sql, parameters));
                connection.Execute(string.Format("DROP DATABASE [{0}]", builder.InitialCatalog));
            }
        }
    }
}
