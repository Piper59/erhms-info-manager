using System;
using System.Data.SqlClient;

namespace ERHMS.Data.SqlServer
{
    public abstract class SqlServerDatabase : Database
    {
        public static string GetConnectionString(
            string dataSource = "",
            string initialCatalog = "",
            bool encrypt = false,
            AuthenticationMode authenticationMode = AuthenticationMode.Windows,
            string userID = "",
            string password = "")
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            if (!string.IsNullOrEmpty(dataSource))
            {
                builder.DataSource = dataSource;
            }
            if (!string.IsNullOrEmpty(initialCatalog))
            {
                builder.InitialCatalog = initialCatalog;
            }
            if (encrypt)
            {
                builder.Encrypt = true;
            }
            switch (authenticationMode)
            {
                case AuthenticationMode.Windows:
                    builder.IntegratedSecurity = true;
                    break;
                case AuthenticationMode.SqlServer:
                    if (!string.IsNullOrEmpty(userID))
                    {
                        builder.UserID = userID;
                    }
                    if (!string.IsNullOrEmpty(password))
                    {
                        builder.Password = password;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(authenticationMode));
            }
            return builder.ConnectionString;
        }

        protected new SqlConnectionStringBuilder ConnectionStringBuilder =>
            (SqlConnectionStringBuilder)base.ConnectionStringBuilder;
        public string Instance => ConnectionStringBuilder.DataSource;
        public override string Name => ConnectionStringBuilder.InitialCatalog;

        protected SqlServerDatabase(string connectionString)
            : base(DatabaseProvider.SqlServer, connectionString) { }

        public override string ToString()
        {
            return $"{Instance}.{Name}";
        }
    }
}
