using System;
using System.Data;

namespace ERHMS.Data
{
    public interface ITransactor : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        void Commit();
    }
}
