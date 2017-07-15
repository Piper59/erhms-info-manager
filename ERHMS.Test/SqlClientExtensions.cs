using System.Data.SqlClient;

namespace ERHMS.Test
{
    public static class SqlClientExtensions
    {
        public static void ExecuteMaster(string connectionString, string sql)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
