using Dapper;
using System.Data;

namespace ERHMS.Data.SqlServer
{
    public class SqlServerUserDatabase : SqlServerDatabase
    {
        public SqlServerMasterDatabase Master { get; }

        public SqlServerUserDatabase(string connectionString)
            : base(connectionString)
        {
            Master = SqlServerMasterDatabase.FromUserConnectionString(connectionString);
        }

        public override bool Exists()
        {
            string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@name", Name }
            };
            using (IDbConnection connection = Master.Connect())
            {
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        protected override void CreateCore()
        {
            string sql = $"CREATE DATABASE {Quote(Name)};";
            using (IDbConnection connection = Master.Connect())
            {
                connection.Execute(sql);
            }
        }

        protected override void DeleteCore()
        {
            string sql = $"DROP DATABASE {Quote(Name)};";
            using (IDbConnection connection = Master.Connect())
            {
                connection.Execute(sql);
            }
        }
    }
}
