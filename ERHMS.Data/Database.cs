﻿using Dapper;
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

            public IDbConnection Connection
            {
                get { return Database.Connection; }
                private set { Database.Connection = value; }
            }

            public Connector(Database database)
            {
                Database = database;
                if (Connection == null)
                {
                    Connection = database.ConnectCore();
                    isOwner = true;
                }
            }

            public void Dispose()
            {
                if (isOwner)
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        private class Transactor : ITransactor
        {
            private readonly IConnector connector;
            private readonly bool isOwner;
            private bool committed;

            public Database Database { get; }
            public IDbConnection Connection => Database.Connection;

            public IDbTransaction Transaction
            {
                get { return Database.Transaction; }
                private set { Database.Transaction = value; }
            }

            public Transactor(Database database)
            {
                Database = database;
                connector = new Connector(database);
                if (Transaction == null)
                {
                    Transaction = Connection.BeginTransaction();
                    isOwner = true;
                }
            }

            public void Commit()
            {
                if (isOwner)
                {
                    Transaction.Commit();
                    committed = true;
                }
            }

            public void Dispose()
            {
                if (isOwner)
                {
                    if (!committed)
                    {
                        Transaction.Rollback();
                    }
                    Transaction.Dispose();
                    Transaction = null;
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

        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            DbConnectionStringBuilder connectionStringBuilder = ProviderFactory.CreateConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = ConnectionString;
            return connectionStringBuilder;
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

        private DbConnection GetBaseConnection()
        {
            DbConnection connection = ProviderFactory.CreateConnection();
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

        public bool TableExists(string tableName)
        {
            using (DbConnection connection = GetBaseConnection())
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables", new string[]
                {
                    null,
                    null,
                    tableName
                });
                return schema.Rows.Count > 0;
            }
        }

        public virtual int GetLastId()
        {
            using (Connect())
            {
                IQuery query = new Query.Literal
                {
                    Sql = "SELECT @@IDENTITY;"
                };
                return ExecuteScalar<int>(query);
            }
        }
    }
}
