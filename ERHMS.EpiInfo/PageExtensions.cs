using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;

namespace ERHMS.EpiInfo
{
    public static class PageExtensions
    {
        public static void DeleteDataTables(this Page @this)
        {
            Log.Default.Debug($"Deleting data tables: {@this.DisplayName}");
            View view = @this.GetView();
            CollectedDataProvider collectedData = @this.GetProject().CollectedData;
            foreach (Field field in @this.Fields)
            {
                if (field is GridField gridField)
                {
                    collectedData.DeleteDataTableForGrid(view, gridField);
                }
            }
            if (collectedData.TableExists(@this.TableName))
            {
                collectedData.DeleteTable(@this.TableName);
            }
        }

        public static void DeleteMetadata(this Page @this)
        {
            Log.Default.Debug($"Deleting metadata: {@this.DisplayName}");
            IMetadataProvider metadata = @this.GetMetadata();
            metadata.DeleteFields(@this);
            metadata.DeletePage(@this);
            metadata.SynchronizePageNumbersOnDelete(@this.GetView(), @this.Position);
        }
    }
}
