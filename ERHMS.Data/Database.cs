using Dapper;
using ERHMS.Common.Logging;
using ERHMS.Data.Logging;
using ERHMS.Data.Querying;
using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public abstract class Database : IDatabase
    {
        private class Connector : IConnector
        {
            private readonly bool isOwner;

            public Database Database { get; }

            public Connector(Database database)
            {
                Database = database;
                if (database.Connection == null)
                {
                    database.Connection = database.ConnectCore();
                    isOwner = true;
                }
            }

            public void Dispose()
            {
                if (isOwner)
                {
                    Database.Connection.Dispose();
                    Database.Connection = null;
                }
            }
        }

        private class Transactor : ITransactor
        {
            private readonly Connector connector;
            private readonly bool isOwner;
            private bool committed;

            public Database Database { get; }

            public Transactor(Database database)
            {
                Database = database;
                connector = new Connector(database);
                if (database.Transaction == null)
                {
                    database.Transaction = database.Connection.BeginTransaction();
                    isOwner = true;
                }
            }

            public void Commit()
            {
                if (isOwner)
                {
                    Database.Transaction.Commit();
                    committed = true;
                }
            }

            public void Dispose()
            {
                if (isOwner)
                {
                    if (!committed)
                    {
                        Database.Transaction.Rollback();
                    }
                    Database.Transaction.Dispose();
                    Database.Transaction = null;
                }
                connector.Dispose();
            }
        }

        public DatabaseProvider Provider { get; }
        protected DbProviderFactory ProviderFactory { get; }
        protected DbConnectionStringBuilder ConnectionStringBuilder { get; }
        public abstract string Name { get; }
        public string ConnectionString => ConnectionStringBuilder.ConnectionString;
        protected IDbConnection Connection { get; private set; }
        protected IDbTransaction Transaction { get; private set; }

        protected Database(DatabaseProvider provider, string connectionString)
        {
            Provider = provider;
            ProviderFactory = provider.ToProviderFactory();
            ConnectionStringBuilder = ProviderFactory.CreateConnectionStringBuilder();
            ConnectionStringBuilder.ConnectionString = connectionString;
        }

        private string Escape(string identifier)
        {
            return identifier.Replace("]", "]]");
        }

        public virtual string Quote(string identifier)
        {
            return $"[{Escape(identifier)}]";
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

        private IDbConnection ConnectCore()
        {
            Log.Instance.Debug($"Connecting to database: {this}");
            IDbConnection connection = GetLoggingConnection();
            connection.Open();
            return connection;
        }

        public IConnector Connect()
        {
            return new Connector(this);
        }

        public ITransactor Transact()
        {
            return new Transactor(this);
        }

        public int Execute(IQuery query)
        {
            return Connection.Execute(query.Sql, query.Parameters, Transaction);
        }

        public TResult ExecuteScalar<TResult>(IQuery query)
        {
            return Connection.ExecuteScalar<TResult>(query.Sql, query.Parameters, Transaction);
        }

        public IDataReader ExecuteReader(IQuery query)
        {
            return Connection.ExecuteReader(query.Sql, query.Parameters, Transaction);
        }

        public virtual int GetLastId()
        {
            IQuery query = new Query.Literal
            {
                Sql = "SELECT @@IDENTITY;"
            };
            using (Connect())
            {
                return ExecuteScalar<int>(query);
            }
        }
    }
}
