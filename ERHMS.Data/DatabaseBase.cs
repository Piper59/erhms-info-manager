using log4net;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class DatabaseBase : IDatabase
    {
        protected static ILog Log { get; } = LogManager.GetLogger(nameof(ERHMS));

        protected static IDbConnection Connect(IDbConnection connection)
        {
            connection = new LoggingConnection(connection, Log);
            connection.Open();
            return connection;
        }

        public abstract DatabaseType Type { get; }
        public abstract DbConnectionStringBuilder ConnectionStringBuilder { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;
        protected abstract DbCommandBuilder CommandBuilder { get; }
        public abstract string Name { get; }

        protected abstract IDbConnection GetConnection();

        public abstract bool Exists();

        public abstract void CreateCore();

        public void Create()
        {
            Log.Debug($"Creating database: {this}");
            CreateCore();
        }

        public IDbConnection Connect()
        {
            return Connect(GetConnection());
        }

        public string Quote(string identifier)
        {
            return CommandBuilder.QuoteIdentifier(identifier);
        }
    }
}
