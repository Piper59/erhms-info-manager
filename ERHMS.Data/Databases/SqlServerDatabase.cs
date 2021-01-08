using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace ERHMS.Data.Databases
{
    public class SqlServerDatabase : Database<SqlConnectionStringBuilder, SqlConnection>
    {
        private readonly SqlConnectionStringBuilder masterConnectionStringBuilder;

        public override DatabaseType Type => DatabaseType.SqlServer;
        public override string Name => ConnectionStringBuilder.InitialCatalog;

        public SqlServerDatabase(string connectionString)
            : base(connectionString)
        {
            masterConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
        }

        protected override string GetId(SqlConnectionStringBuilder connectionStringBuilder)
        {
            return $"{connectionStringBuilder.DataSource}.{connectionStringBuilder.InitialCatalog}";
        }

        public override bool Exists()
        {
            using (IDbConnection connection = Connect(masterConnectionStringBuilder))
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
            using (IDbConnection connection = Connect(masterConnectionStringBuilder))
            {
                string sql = $"CREATE DATABASE {Quote(Name)};";
                connection.Execute(sql);
            }
        }
    }
}
