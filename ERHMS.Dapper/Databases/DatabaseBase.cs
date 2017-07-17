using System;
using System.Data;

namespace ERHMS.Dapper
{
    public abstract class DatabaseBase : IDatabase
    {
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
            transaction = new Transaction(connection.BeginTransaction());
            transaction.Closed += (sender, e) =>
            {
                connection.Dispose();
                transaction = null;
            };
            return transaction;
        }

        public T Invoke<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            IDbConnection connection = transaction?.Connection ?? GetConnection();
            try
            {
                return action(connection, transaction?.Base);
            }
            finally
            {
                if (transaction == null)
                {
                    connection.Dispose();
                }
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
    }
}
