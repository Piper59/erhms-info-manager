using ERHMS.Data;

namespace ERHMS.EpiInfo.Data
{
    public static class IDatabaseExtensions
    {
        public static bool IsInitialized(this IDatabase @this)
        {
            return @this.TableExists(TableNames.DbInfo);
        }
    }
}
