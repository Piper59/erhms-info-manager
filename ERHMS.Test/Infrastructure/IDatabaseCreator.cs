using ERHMS.Dapper;
using System.Data;

namespace ERHMS.Test
{
    public interface IDatabaseCreator
    {
        void SetUp();
        void TearDown();
        IDbConnection GetConnection();
        IDatabase GetDatabase();
    }
}
