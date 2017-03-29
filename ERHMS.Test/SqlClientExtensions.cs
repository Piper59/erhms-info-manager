using System.Data.SqlClient;
using System.Linq;

namespace ERHMS.Test
{
    public static class SqlClientExtensions
    {
        public static void ExecuteMaster(string connectionString, string format, params string[] identifiers)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder();
            object[] quotedIdentifiers = identifiers.Select(identifier => commandBuilder.QuoteIdentifier(identifier)).ToArray();
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            using (SqlCommand command = new SqlCommand(string.Format(format, quotedIdentifiers), connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
