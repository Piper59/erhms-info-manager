using ERHMS.Common.Logging;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data.Databases
{
    public abstract class Database<TConnectionStringBuilder, TConnection> : IDatabase
        where TConnectionStringBuilder : DbConnectionStringBuilder, new()
        where TConnection : DbConnection, new()
    {
        public abstract DatabaseType Type { get; }
        protected TConnectionStringBuilder ConnectionStringBuilder { get; }
        public string Id => GetId(ConnectionStringBuilder);
        public abstract string Name { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;

        protected Database(string connectionString)
        {
            ConnectionStringBuilder = new TConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
        }

        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new TConnectionStringBuilder
            {
                ConnectionString = ConnectionString
            };
        }

        protected abstract string GetId(TConnectionStringBuilder connectionStringBuilder);
        public abstract bool Exists();
        protected abstract void CreateCore();

        public void Create()
        {
            Log.Instance.Debug($"Creating database: {Id}");
            CreateCore();
        }

        protected IDbConnection Connect(TConnectionStringBuilder connectionStringBuilder)
        {
            IDbConnection connection = new LoggingConnection(GetId(connectionStringBuilder), new TConnection())
            {
                ConnectionString = connectionStringBuilder.ConnectionString
            };
            connection.Open();
            return connection;
        }

        public IDbConnection Connect()
        {
            return Connect(ConnectionStringBuilder);
        }

        public virtual string Quote(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }
    }
}
