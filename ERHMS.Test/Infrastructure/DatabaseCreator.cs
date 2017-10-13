using ERHMS.Dapper;
using ERHMS.Utility;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Reflection;

namespace ERHMS.Test
{
    public interface IDatabaseCreator
    {
        void SetUp();
        void TearDown();
        IDbConnection GetConnection();
        IDatabase GetDatabase();
    }

    public class AccessDatabaseCreator : IDatabaseCreator
    {
        public TempDirectory Directory { get; private set; }
        public OleDbConnectionStringBuilder Builder { get; private set; }

        public AccessDatabaseCreator(string name)
        {
            Directory = new TempDirectory(name);
            Builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.Providers.Jet4,
                DataSource = Directory.CombinePaths(name + OleDbExtensions.FileExtensions.Access)
            };
        }

        public void SetUp()
        {
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", Builder.DataSource);
        }

        public void TearDown()
        {
            Directory.Dispose();
        }

        public IDbConnection GetConnection()
        {
            return new OleDbConnection(Builder.ConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return new AccessDatabase(Builder);
        }
    }

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
