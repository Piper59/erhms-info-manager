using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

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

        public static int ExecuteNonQuery(this SqlConnection @this, string format, params string[] identifiers)
        {
            string sql;
            using (SqlCommandBuilder builder = new SqlCommandBuilder())
            {
                sql = string.Format(format, identifiers.Select(identifier => builder.QuoteIdentifier(identifier)).ToArray());
            }
            using (SqlCommand command = new SqlCommand(sql, @this))
            {
                return command.ExecuteNonQuery();
            }
        }
    }
}
