using Epi;
using Epi.Data.Services;
using ERHMS.Utility;

namespace ERHMS.EpiInfo
{
    public static class CollectedDataProviderExtensions
    {
        public static void EnsureDataTablesExist(this CollectedDataProvider @this, int viewId)
        {
            Log.Logger.DebugFormat("Ensuring data tables exist: {0}", viewId);
            View view = @this.Project.Metadata.GetViewById(viewId);
            if (!@this.TableExists(view.TableName))
            {
                @this.CreateDataTableForView(view, 1);
            }
        }
    }
}
