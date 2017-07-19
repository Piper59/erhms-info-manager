using System;
using System.Data;

namespace ERHMS.Dapper
{
    public interface IDatabase
    {
        bool Exists();
        void Create();
        IDbConnection GetConnection();
        Transaction BeginTransaction();
        T Invoke<T>(Func<IDbConnection, IDbTransaction, T> action);
        void Invoke(Action<IDbConnection, IDbTransaction> action);
        T Transact<T>(Func<IDbConnection, IDbTransaction, T> action);
        void Transact(Action<IDbConnection, IDbTransaction> action);
    }
}
