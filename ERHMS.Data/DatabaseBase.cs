using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class DatabaseBase : IDatabase
    {
        protected static IDbConnection Connect(IDbConnection connection)
        {
            connection = new LoggingConnection(connection, Log.Default);
            connection.Open();
            return connection;
        }

        public abstract DatabaseType Type { get; }
        public abstract DbConnectionStringBuilder Builder { get; }
        public string ConnectionString => Builder.ConnectionString;
        protected abstract DbCommandBuilder CommandBuilder { get; }
        public abstract string Name { get; }

        protected abstract IDbConnection GetConnection();
        public abstract bool Exists();
        public abstract void Create();

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
