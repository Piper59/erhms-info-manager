using Dapper;
using ERHMS.Utility;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;

namespace ERHMS.Test.Dapper
{
    public static class DatabaseTestBase
    {
        public static class Access
        {
            public static void SetUp(string name, out TempDirectory directory, out OleDbConnectionStringBuilder builder)
            {
                directory = new TempDirectory(name);
                string path = directory.CombinePaths(name + ".mdb");
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", path);
                builder = new OleDbConnectionStringBuilder
                {
                    Provider = OleDbExtensions.AccessProvider,
                    DataSource = path
                };
            }
        }

        public static class SqlServer
        {
            private static SqlConnectionStringBuilder GetBuilder()
            {
                return new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString)
                {
                    Pooling = false
                };
            }

            public static void SetUp(out SqlConnectionStringBuilder builder)
            {
                builder = GetBuilder();
                using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
                {
                    connection.Execute(string.Format("CREATE DATABASE [{0}]", builder.InitialCatalog));
                }
            }

            public static void TearDown()
            {
                SqlConnectionStringBuilder builder = GetBuilder();
                using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
                {
                    connection.Execute(string.Format("DROP DATABASE [{0}]", builder.InitialCatalog));
                }
            }
        }
    }
}
