using ERHMS.Data;

namespace ERHMS.EpiInfo.Data
{
    public static class IDatabaseExtensions
    {
        public static bool IsInitialized(this IDatabase @this)
        {
            return @this.TableExists(TableNames.DbInfo);
        }

        public static DatabaseStatus GetStatus(this IDatabase @this)
        {
            if (@this.Exists())
            {
                return @this.IsInitialized() ? DatabaseStatus.Initialized : DatabaseStatus.Existing;
            }
            else
            {
                return DatabaseStatus.Missing;
            }
        }
    }
}
