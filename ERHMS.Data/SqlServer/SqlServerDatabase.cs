using System.Data.SqlClient;

namespace ERHMS.Data.SqlServer
{
    public abstract class SqlServerDatabase : Database
    {
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
