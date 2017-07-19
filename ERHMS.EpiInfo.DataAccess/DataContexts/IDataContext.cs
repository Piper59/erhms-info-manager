using ERHMS.Dapper;

namespace ERHMS.EpiInfo.DataAccess
{
    public interface IDataContext
    {
        IDatabase Database { get; }
        Project Project { get; }
    }
}
