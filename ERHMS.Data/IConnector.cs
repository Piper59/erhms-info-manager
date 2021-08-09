using System;
using System.Data;

namespace ERHMS.Data
{
    public interface IConnector : IDisposable
    {
        IDbConnection Connection { get; }
    }
}
