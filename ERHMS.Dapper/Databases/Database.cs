using System;
using System.Data;
using System.Data.Common;

namespace ERHMS.Dapper
{
    public abstract class Database : IDatabase
    {
        protected static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
        }

        public abstract DbConnectionStringBuilder Builder { get; }
        public abstract string Name { get; }

        private Transaction transaction;

        public abstract bool Exists();
        public abstract void Create();
        protected abstract IDbConnection GetConnectionInternal();

        public IDbConnection GetConnection()
        {
            return new LoggingConnection(GetConnectionInternal());
        }

        public Transaction BeginTransaction()
        {
            if (transaction != null)
            {
                throw new InvalidOperationException("Parallel transactions are not supported.");
            }
            IDbConnection connection = GetConnection();
            connection.Open();
            transaction = new Transaction(connection);
            transaction.Closed += (sender, e) =>
            {
                connection.Dispose();
                transaction = null;
            };
            return transaction;
        }

        public T Invoke<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            if (transaction == null)
            {
                using (IDbConnection connection = GetConnection())
                {
                    return action(connection, null);
                }
            }
            else
            {
                return action(transaction.Connection, transaction.Base);
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

        public T Transact<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            if (transaction == null)
            {
                using (Transaction transaction = BeginTransaction())
                {
                    T result = action(transaction.Connection, transaction.Base);
                    transaction.Commit();
                    return result;
                }
            }
            else
            {
                return action(transaction.Connection, transaction.Base);
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
