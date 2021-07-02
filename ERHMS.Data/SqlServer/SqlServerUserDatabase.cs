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
            using (Master.Connect())
            {
                IQuery query = new Query.Literal
                {
                    Sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;",
                    Parameters = new ParameterCollection
                    {
                        { "@name", Name }
                    }
                };
                return Master.ExecuteScalar<int>(query) > 0;
            }
        }

        protected override void CreateCore()
        {
            using (Master.Connect())
            {
                IQuery query = new Query.Literal
                {
                    Sql = $"CREATE DATABASE {Quote(Name)};"
                };
                Master.Execute(query);
            }
        }

        protected override void DeleteCore()
        {
            using (Master.Connect())
            {
                IQuery query = new Query.Literal
                {
                    Sql = $"DROP DATABASE {Quote(Name)};"
                };
                Master.Execute(query);
            }
        }
    }
}
