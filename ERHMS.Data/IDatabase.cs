using System.Data;

namespace ERHMS.Data
{
    public interface IDatabase
    {
        DatabaseProvider Provider { get; }
        string Name { get; }
        string ConnectionString { get; }

        bool Exists();
        void Create();
        IDbConnection Connect();
        string Quote(string identifier);
    }
}
