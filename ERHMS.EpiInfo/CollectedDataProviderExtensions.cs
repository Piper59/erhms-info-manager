using Epi;
using Epi.Data.Services;

namespace ERHMS.EpiInfo
{
    public static class CollectedDataProviderExtensions
    {
        public static void EnsureDataTablesExist(this CollectedDataProvider @this, View view)
        {
            if (!@this.TableExists(view.TableName))
            {
                @this.CreateDataTableForView(view, 1);
            }
        }
    }
}
