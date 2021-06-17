using ERHMS.Data.Querying;

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
            IQuery query = new Query.Literal
            {
                Sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;",
                Parameters = new ParameterCollection
                {
                    { "@name", Name }
                }
            };
            using (Master.Connect())
            {
                return Master.ExecuteScalar<int>(query) > 0;
            }
        }

        protected override void CreateCore()
        {
            IQuery query = new Query.Literal
            {
                Sql = $"CREATE DATABASE {Quote(Name)};"
            };
            using (Master.Connect())
            {
                Master.Execute(query);
            }
        }

        protected override void DeleteCore()
        {
            IQuery query = new Query.Literal
            {
                Sql = $"DROP DATABASE {Quote(Name)};"
            };
            using (Master.Connect())
            {
                Master.Execute(query);
            }
        }
    }
}
