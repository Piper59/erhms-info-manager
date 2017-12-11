using ERHMS.Dapper;
using ERHMS.Utility;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
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
        public static AccessDatabaseCreator ForName(string name)
        {
            TempDirectory directory = new TempDirectory(name);
            string path = directory.CombinePaths(name + OleDbExtensions.FileExtensions.Access);
            AccessDatabaseCreator creator = new AccessDatabaseCreator(path)
            {
                directory = directory
            };
            return creator;
        }

        public static AccessDatabaseCreator ForPath(string path)
        {
            return new AccessDatabaseCreator(path);
        }

        private TempDirectory directory;

        public OleDbConnectionStringBuilder Builder { get; private set; }

        private AccessDatabaseCreator(string path)
        {
            Builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.Providers.Jet4,
                DataSource = path
            };
        }

        public void SetUp()
        {
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", Builder.DataSource);
        }

        public void TearDown()
        {
            if (directory == null)
            {
                File.Delete(Builder.DataSource);
            }
            else
            {
                directory.Dispose();
            }
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

        public SqlServerDatabaseCreator(string name = null)
        {
            Builder = Config.GetTestConnectionStringBuilder();
            if (name != null)
            {
                Builder.InitialCatalog = name;
            }
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
