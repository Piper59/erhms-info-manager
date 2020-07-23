using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public class SqlServerDatabase : Database
    {
        private const string MasterDatabaseName = "master";

        private SqlConnectionStringBuilder builder;
        private SqlCommandBuilder commandBuilder = new SqlCommandBuilder();

        public override DatabaseType Type => DatabaseType.SqlServer;
        public override DbConnectionStringBuilder Builder => builder;
        protected override DbCommandBuilder CommandBuilder => commandBuilder;
        public string Instance => builder.DataSource;
        public override string Name => builder.InitialCatalog;

        public SqlServerDatabase(string connectionString)
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }

        protected override IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        private IDbConnection GetMasterConnection()
        {
            DbConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = MasterDatabaseName
            };
            return new SqlConnection(builder.ConnectionString);
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

        public override string ToString()
        {
            if (Name == null)
            {
                return null;
            }
            if (Instance == null)
            {
                return Name;
            }
            return $"{Instance}.{Name}";
        }
    }
}
