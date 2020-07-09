using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public class SqlServerDatabase : DatabaseBase
    {
        private SqlConnectionStringBuilder connectionStringBuilder;
        private SqlCommandBuilder commandBuilder = new SqlCommandBuilder();

        public override DatabaseType Type => DatabaseType.SqlServer;
        public override DbConnectionStringBuilder ConnectionStringBuilder => connectionStringBuilder;
        protected override DbCommandBuilder CommandBuilder => commandBuilder;
        public string Instance => connectionStringBuilder.DataSource;
        public override string Name => connectionStringBuilder.InitialCatalog;

        public SqlServerDatabase(string connectionString)
        {
            connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        }

        protected override DbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        private DbConnection GetDefaultConnection()
        {
            DbConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString)
            {
                InitialCatalog = null
            };
            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }

        public override bool Exists()
        {
            using (IDbConnection connection = Connect(GetDefaultConnection()))
            {
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", Name);
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        public override void CreateCore()
        {
            using (IDbConnection connection = Connect(GetDefaultConnection()))
            {
                string sql = string.Format("CREATE DATABASE {0};", Quote(Name));
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
