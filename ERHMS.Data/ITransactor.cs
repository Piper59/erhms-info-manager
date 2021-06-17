using System;

namespace ERHMS.Data
{
    public interface ITransactor : IDisposable
    {
        void Commit();
    }
}
