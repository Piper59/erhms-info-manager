using System;
using System.Data.SqlClient;

namespace ERHMS.Data.SqlServer
{
    public class SqlServerMasterDatabase : SqlServerDatabase
    {
        public static SqlServerMasterDatabase FromMasterConnectionString(string connectionString)
        {
            return new SqlServerMasterDatabase(connectionString);
        }

        public static SqlServerMasterDatabase FromUserConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            return new SqlServerMasterDatabase(connectionStringBuilder.ConnectionString);
        }

        private SqlServerMasterDatabase(string connectionString)
            : base(connectionString) { }

        public override bool Exists()
        {
            return true;
        }

        protected override void CreateCore()
        {
            throw new NotSupportedException("The master database cannot be created.");
        }

        protected override void DeleteCore()
        {
            throw new NotSupportedException("The master database cannot be deleted.");
        }
    }
}
