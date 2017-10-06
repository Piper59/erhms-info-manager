using ERHMS.Dapper;
using ERHMS.Utility;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ERHMS.Test
{
    public class SqlServerDatabaseCreator : IDatabaseCreator
    {
        public SqlConnectionStringBuilder Builder { get; private set; }

        public SqlServerDatabaseCreator()
        {
            Builder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString)
            {
                Pooling = false
            };
        }

        private void ExecuteNonQuery(string format, params string[] identifiers)
        {
            using (SqlConnection connection = SqlClientExtensions.GetMasterConnection(Builder.ConnectionString))
            {
                connection.Open();
                connection.ExecuteNonQuery(format, identifiers);
            }
        }

        public void SetUp()
        {
            ExecuteNonQuery("CREATE DATABASE {0}", Builder.InitialCatalog);
        }

        public void TearDown()
        {
            ExecuteNonQuery("DROP DATABASE {0}", Builder.InitialCatalog);
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(Builder.ConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return new SqlServerDatabase(Builder);
        }
    }
}
