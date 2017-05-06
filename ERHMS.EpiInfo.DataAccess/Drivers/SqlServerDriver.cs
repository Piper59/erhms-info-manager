using System.Data.SqlClient;

namespace ERHMS.EpiInfo.DataAccess
{
    public class SqlServerDriver : DataDriverBase
    {
        public static SqlServerDriver Create(string dataSource, string initialCatalog, bool encrypt, string userId = null, string password = null)
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
            return new SqlServerDriver(builder);
        }

        public static SqlServerDriver Create(Project project)
        {
            return new SqlServerDriver(new SqlConnectionStringBuilder(project.CollectedDataConnectionString));
        }

        public new SqlConnectionStringBuilder Builder { get; private set; }

        public SqlServerDriver(SqlConnectionStringBuilder builder)
            : base(DataProvider.SqlServer, builder, builder.InitialCatalog)
        {
            Builder = builder;
        }

        public override string GetParameterName(int index)
        {
            return string.Format("@p{0}", index);
        }

        private SqlConnection OpenMasterConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(Builder.ConnectionString)
            {
                InitialCatalog = "master"
            };
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            return connection;
        }

        public override bool DatabaseExists()
        {
            string sql = "SELECT TOP 1 [name] FROM [sys].[databases] WHERE [name] = @name";
            using (SqlConnection connection = OpenMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", Builder.InitialCatalog);
                return command.ExecuteScalar() != null;
            }
        }

        public override void CreateDatabase()
        {
            string sql = string.Format("CREATE DATABASE {0}", Escape(Builder.InitialCatalog));
            using (SqlConnection connection = OpenMasterConnection())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public override bool TableExists(string tableName)
        {
            string sql = "SELECT TOP 1 [name] FROM [sys].[tables] WHERE [name] = @name";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@name", tableName);
                return command.ExecuteScalar() != null;
            }
        }
    }
}
