using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class Database : IDatabase
    {
        protected static IDbConnection Connect(IDbConnection connection)
        {
            connection = new LoggingConnection(connection, Log.Default);
            connection.Open();
            return connection;
        }

        public abstract DatabaseType Type { get; }
        public abstract DbConnectionStringBuilder ConnectionStringBuilder { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;
        public abstract string Name { get; }

        public abstract bool Exists();
        protected abstract void CreateCore();

        public void Create()
        {
            Log.Default.Debug($"Creating database: {this}");
            CreateCore();
        }

        protected abstract IDbConnection GetConnection();

        public IDbConnection Connect()
        {
            return Connect(GetConnection());
        }

        public virtual string Quote(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }
    }
}
