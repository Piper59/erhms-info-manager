using System.Data.SqlClient;

namespace ERHMS.EpiInfo.DataAccess
{
    public class SqlServerDriver : DataDriverBase
    {
        public static SqlServerDriver Create(string dataSource, string initialCatalog, string userId = null, string password = null)
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
            return new SqlServerDriver(builder);
        }

        public static SqlServerDriver Create(Project project)
        {
            return new SqlServerDriver(new SqlConnectionStringBuilder(project.CollectedDataConnectionString));
        }

        public new SqlConnectionStringBuilder Builder { get; private set; }

        private SqlServerDriver(SqlConnectionStringBuilder builder)
            : base(DataProvider.SqlServer, builder, builder.InitialCatalog)
        {
            Builder = builder;
        }

        public override string GetParameterName(int index)
        {
            return string.Format("@p{0}", index);
        }

        private SqlConnection GetMasterConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Builder.ConnectionString);
            builder.InitialCatalog = "master";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            return connection;
        }

        public override bool DatabaseExists()
        {
            string sql = "SELECT 1 FROM [sys].[databases] WHERE [name] = @name";
            using (SqlConnection connection = GetMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", Builder.InitialCatalog);
                return command.ExecuteScalar() != null;
            }
        }

        public override void CreateDatabase()
        {
            string sql = string.Format("CREATE DATABASE {0}", Escape(Builder.InitialCatalog));
            using (SqlConnection connection = GetMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
