using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class Database : IDatabase
    {
        private readonly DbProviderFactory providerFactory;

        public DatabaseProvider Provider { get; }
        protected DbConnectionStringBuilder ConnectionStringBuilder { get; }
        protected DbCommandBuilder CommandBuilder { get; }
        public abstract string Name { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;

        protected Database(DatabaseProvider provider, string connectionString)
        {
            Provider = provider;
            providerFactory = provider.ToProviderFactory();
            ConnectionStringBuilder = providerFactory.CreateConnectionStringBuilder();
            ConnectionStringBuilder.ConnectionString = connectionString;
            CommandBuilder = providerFactory.CreateCommandBuilder();
        }

        public abstract bool Exists();
        protected abstract void CreateCore();

        public void Create()
        {
            Log.Default.Debug($"Creating database: {this}");
            CreateCore();
        }

        private IDbConnection GetBaseConnection()
        {
            IDbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        private IDbConnection GetLoggingConnection()
        {
            return new LoggingConnection(GetBaseConnection());
        }

        public IDbConnection Connect()
        {
            Log.Default.Debug($"Connecting to database: {this}");
            IDbConnection connection = GetLoggingConnection();
            connection.Open();
            return connection;
        }

        public string Quote(string identifier)
        {
            return CommandBuilder.QuoteIdentifier(identifier);
        }
    }
}
