using System.Data.Common;
using System.Data.SqlClient;

namespace ERHMS.Utility
{
    public static class SqlClientExtensions
    {
        public static SqlConnection GetMasterConnection(string connectionString)
        {
            DbConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            return new SqlConnection(builder.ConnectionString);
        }
    }
}
