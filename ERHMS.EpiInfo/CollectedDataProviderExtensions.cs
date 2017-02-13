using Epi;
using Epi.Data.Services;
using ERHMS.Utility;

namespace ERHMS.EpiInfo
{
    public static class CollectedDataProviderExtensions
    {
        public static void EnsureDataTablesExist(this CollectedDataProvider @this, View view)
        {
            Log.Logger.DebugFormat("Ensuring data tables exist: {0}", view.Name);
            if (!@this.TableExists(view.TableName))
            {
                @this.CreateDataTableForView(view, 1);
            }
        }
    }
}
