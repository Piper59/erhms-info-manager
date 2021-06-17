using ERHMS.Data.Querying;
using System.Data;

namespace ERHMS.Data
{
    public interface IDatabase
    {
        DatabaseProvider Provider { get; }
        string Name { get; }
        string ConnectionString { get; }

        string Quote(string identifier);
        bool Exists();
        void Create();
        void Delete();
        IConnector Connect();
        ITransactor Transact();
        int Execute(IQuery query);
        TResult ExecuteScalar<TResult>(IQuery query);
        IDataReader ExecuteReader(IQuery query);
        int GetLastId();
    }
}
