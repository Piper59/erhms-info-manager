using Dapper;
using ERHMS.Utility;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ERHMS.Dapper
{
    public class SqlServerDatabase : Database
    {
        public static SqlServerDatabase Construct(string dataSource, string initialCatalog, bool encrypt = false, string userId = null, string password = null)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.InitialCatalog = initialCatalog;
            builder.Encrypt = encrypt;
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

        private SqlConnectionStringBuilder builder;
        public override DbConnectionStringBuilder Builder
        {
            get { return builder; }
        }

        public override string Name
        {
            get { return builder.InitialCatalog; }
        }

        public SqlServerDatabase(SqlConnectionStringBuilder builder)
        {
            this.builder = builder;
        }

        public SqlServerDatabase(string connectionString)
            : this(new SqlConnectionStringBuilder(connectionString)) { }

        public override bool Exists()
        {
            using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", builder.InitialCatalog);
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        public override void Create()
        {
            using (IDbConnection connection = SqlClientExtensions.GetMasterConnection(builder.ConnectionString))
            {
                string sql = string.Format("CREATE DATABASE {0}", Escape(builder.InitialCatalog));
                connection.Execute(sql);
            }
        }

        public override bool TableExists(string name)
        {
            return Invoke((connection, transaction) =>
            {
                string sql = "SELECT COUNT(*) FROM sys.tables WHERE name = @name";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", name);
                return connection.ExecuteScalar<int>(sql, parameters, transaction) > 0;
            });
        }

        protected override IDbConnection GetConnectionInternal()
        {
            return new SqlConnection(builder.ConnectionString);
        }
    }
}
