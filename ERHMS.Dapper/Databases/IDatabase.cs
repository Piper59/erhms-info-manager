using System;
using System.Data;
using System.Data.Common;

namespace ERHMS.Dapper
{
    public interface IDatabase
    {
        DbConnectionStringBuilder Builder { get; }
        string Name { get; }

        bool Exists();
        void Create();
        bool TableExists(string name);
        T Invoke<T>(Func<IDbConnection, IDbTransaction, T> action);
        void Invoke(Action<IDbConnection, IDbTransaction> action);
        T Transact<T>(Func<IDbConnection, IDbTransaction, T> action);
        void Transact(Action<IDbConnection, IDbTransaction> action);
    }
}
