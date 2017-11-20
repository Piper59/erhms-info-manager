using ERHMS.Utility;
using System;
using System.Data;
using System.Data.Common;

namespace ERHMS.Dapper
{
    public abstract class Database : IDatabase
    {
        protected static string Escape(string identifier)
        {
            return DbExtensions.Escape(identifier);
        }

        public abstract DbConnectionStringBuilder Builder { get; }
        public abstract string Name { get; }

        private IDbConnection connection;
        private IDbTransaction transaction;

        public abstract bool Exists();
        public abstract void Create();
        public abstract bool TableExists(string name);
        protected abstract IDbConnection GetConnectionInternal();

        protected IDbConnection GetConnection()
        {
            return new LoggingConnection(GetConnectionInternal());
        }

        public T Invoke<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            if (connection == null)
            {
                try
                {
                    using (connection = GetConnection())
                    {
                        return action(connection, null);
                    }
                }
                finally
                {
                    connection = null;
                }
            }
            else
            {
                return action(connection, transaction);
            }
        }

        public void Invoke(Action<IDbConnection, IDbTransaction> action)
        {
            Invoke((connection, transaction) =>
            {
                action(connection, transaction);
                return (object)null;
            });
        }

        private T TransactInternal<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            try
            {
                using (transaction = connection.BeginTransaction())
                {
                    try
                    {
                        T result = action(connection, transaction);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            finally
            {
                transaction = null;
            }
        }

        public T Transact<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            if (connection == null)
            {
                try
                {
                    using (connection = GetConnection())
                    {
                        connection.Open();
                        return TransactInternal(action);
                    }
                }
                finally
                {
                    connection = null;
                }
            }
            else if (transaction == null)
            {
                using (new ConnectionOpener(connection))
                {
                    return TransactInternal(action);
                }
            }
            else
            {
                return action(connection, transaction);
            }
        }

        public void Transact(Action<IDbConnection, IDbTransaction> action)
        {
            Transact((connection, transaction) =>
            {
                action(connection, transaction);
                return (object)null;
            });
        }
    }
}
