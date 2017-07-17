using Dapper;
using ERHMS.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ERHMS.Dapper
{
    public class SqlServerDatabase : DatabaseBase
    {
        public static SqlServerDatabase Construct(string dataSource, string initialCatalog, string userId = null, string password = null)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.InitialCatalog = initialCatalog;
            if (userId == null && password == null)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                if (userId != null)
                {
                    builder.UserID = userId;
                }
                if (password != null)
                {
                    builder.Password = password;
                }
            }
            return new SqlServerDatabase(builder);
        }

        public SqlConnectionStringBuilder Builder { get; private set; }

        public SqlServerDatabase(SqlConnectionStringBuilder builder)
        {
            Builder = builder;
        }

        public override bool Exists()
        {
            using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(Builder.ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", Builder.InitialCatalog);
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        public override void Create()
        {
            using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(Builder.ConnectionString))
            {
                string sql = string.Format("CREATE DATABASE {0}", IDbConnectionExtensions.Escape(Builder.InitialCatalog));
                connection.Execute(sql);
            }
        }

        protected override IDbConnection GetConnectionInternal()
        {
            return new SqlConnection(Builder.ConnectionString);
        }
    }
}
