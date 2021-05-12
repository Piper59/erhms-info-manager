using ERHMS.Common.Logging;
using ERHMS.Data.Logging;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class Database : IDatabase
    {
        public DatabaseProvider Provider { get; }
        protected DbProviderFactory ProviderFactory { get; }
        protected DbConnectionStringBuilder ConnectionStringBuilder { get; }
        protected DbCommandBuilder CommandBuilder { get; }
        public abstract string Name { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;

        protected Database(DatabaseProvider provider, string connectionString)
        {
            Provider = provider;
            ProviderFactory = provider.ToProviderFactory();
            ConnectionStringBuilder = ProviderFactory.CreateConnectionStringBuilder();
            ConnectionStringBuilder.ConnectionString = connectionString;
            CommandBuilder = ProviderFactory.CreateCommandBuilder();
        }

        public abstract bool Exists();
        protected abstract void CreateCore();
        protected abstract void DeleteCore();

        public void Create()
        {
            Log.Instance.Debug($"Creating database: {this}");
            CreateCore();
        }

        public void Delete()
        {
            Log.Instance.Debug($"Deleting database: {this}");
            DeleteCore();
        }

        private IDbConnection GetBaseConnection()
        {
            IDbConnection connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        private IDbConnection GetLoggingConnection()
        {
            return new LoggingConnection(GetBaseConnection());
        }

        public IDbConnection Connect()
        {
            Log.Instance.Debug($"Connecting to database: {this}");
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
