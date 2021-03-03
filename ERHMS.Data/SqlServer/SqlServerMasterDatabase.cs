using System;
using System.Data.SqlClient;

namespace ERHMS.Data.SqlServer
{
    public class SqlServerMasterDatabase : SqlServerDatabase
    {
        public static class Constants
        {
            public const string InitialCatalog = "master";
        }

        public static SqlServerMasterDatabase FromMasterConnectionString(string connectionString)
        {
            return new SqlServerMasterDatabase(connectionString);
        }

        public static SqlServerMasterDatabase FromUserConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = Constants.InitialCatalog
            };
            return new SqlServerMasterDatabase(connectionStringBuilder.ConnectionString);
        }

        private SqlServerMasterDatabase(string connectionString)
            : base(connectionString) { }

        public override bool Exists()
        {
            throw new NotSupportedException();
        }

        protected override void CreateCore()
        {
            throw new NotSupportedException();
        }

        protected override void DeleteCore()
        {
            throw new NotSupportedException();
        }
    }
}
