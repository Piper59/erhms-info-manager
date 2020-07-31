using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public class SqlServerDatabase : Database
    {
        private const string MasterDatabaseName = "master";

        private readonly SqlConnectionStringBuilder connectionStringBuilder;

        public override DatabaseType Type => DatabaseType.SqlServer;
        protected override DbConnectionStringBuilder ConnectionStringBuilder => connectionStringBuilder;
        public string Instance => connectionStringBuilder.DataSource;
        public override string Name => connectionStringBuilder.InitialCatalog;

        public SqlServerDatabase(string connectionString)
        {
            connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        }

        private IDbConnection GetMasterConnection()
        {
            DbConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = MasterDatabaseName
            };
            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        public override bool Exists()
        {
            using (IDbConnection connection = Connect(GetMasterConnection()))
            {
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;";
                ParameterCollection parameters = new ParameterCollection
                {
                    { "@name", Name }
                };
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        protected override void CreateCore()
        {
            using (IDbConnection connection = Connect(GetMasterConnection()))
            {
                string sql = $"CREATE DATABASE {Quote(Name)};";
                connection.Execute(sql);
            }
        }

        protected override IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public override string ToString()
        {
            if (Name == "")
            {
                return Instance;
            }
            else
            {
                return $"{Instance}.{Name}";
            }
        }
    }
}
