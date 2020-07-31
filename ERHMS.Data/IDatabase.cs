using System.Data;
using System.Data.Common;

namespace ERHMS.Data
{
    public interface IDatabase
    {
        DatabaseType Type { get; }
        string ConnectionString { get; }
        string Name { get; }

        DbConnectionStringBuilder GetConnectionStringBuilder();
        bool Exists();
        void Create();
        IDbConnection Connect();
        string Quote(string identifier);
    }
}
