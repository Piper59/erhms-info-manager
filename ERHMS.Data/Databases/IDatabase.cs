using System.Data;
using System.Data.Common;

namespace ERHMS.Data.Databases
{
    public interface IDatabase
    {
        DatabaseType Type { get; }
        string Id { get; }
        string Name { get; }
        string ConnectionString { get; }

        DbConnectionStringBuilder GetConnectionStringBuilder();
        bool Exists();
        void Create();
        IDbConnection Connect();
        string Quote(string identifier);
    }
}
