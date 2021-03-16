using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;

namespace ERHMS.EpiInfo
{
    public static class PageExtensions
    {
        public static void DeleteData(this Page @this)
        {
            Log.Instance.Debug($"Deleting page data: {@this.DisplayName}");
            View view = @this.GetView();
            CollectedDataProvider collectedData = view.Project.CollectedData;
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
            Log.Instance.Debug($"Deleting page metadata: {@this.DisplayName}");
            IMetadataProvider metadata = @this.GetMetadata();
            metadata.DeleteFields(@this);
            metadata.DeletePage(@this);
        }
    }
}
