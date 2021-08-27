using ERHMS.Data;

namespace ERHMS.Console.Utilities
{
    public class CreateDatabase : Utility
    {
        public DatabaseProvider DatabaseProvider { get; }
        public string ConnectionString { get; }

        public CreateDatabase(DatabaseProvider databaseProvider, string connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        public override void Run()
        {
            IDatabase database = DatabaseProvider.ToDatabase(ConnectionString);
            database.Create();
        }
    }
}
