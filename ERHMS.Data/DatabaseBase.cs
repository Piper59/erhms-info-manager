using ERHMS.Utility;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class DatabaseBase : IDatabase
    {
        protected static IDbConnection Connect(IDbConnection connection)
        {
            connection = new LoggingConnection(connection);
            connection.Open();
            return connection;
        }

        public abstract DatabaseType Type { get; }
        public abstract DbConnectionStringBuilder ConnectionStringBuilder { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;
        protected abstract DbCommandBuilder CommandBuilder { get; }
        public abstract string Name { get; }

        protected abstract DbConnection GetConnection();

        public abstract bool Exists();

        public bool TableExists(string name)
        {
            return GetConnection().TableExists(name);
        }

        public abstract void CreateCore();

        public void Create()
        {
            Log.Default.Debug($"Creating database: {this}");
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
