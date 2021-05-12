using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common.Logging;

namespace ERHMS.EpiInfo.Data
{
    public static class CollectedDataProviderExtensions
    {
        public static void SynchronizeView(this CollectedDataProvider @this, View view)
        {
            Log.Instance.Debug($"Synchronizing view schema: {view.DisplayName}");
            @this.SynchronizeDataTable(view);
        }

        public static void SynchronizeViewTree(this CollectedDataProvider @this, View view)
        {
            @this.SynchronizeView(view);
            foreach (View descendantView in view.GetDescendantViews())
            {
                @this.SynchronizeView(descendantView);
            }
        }

        public static void DeletePage(this CollectedDataProvider @this, Page page)
        {
            Log.Instance.Debug($"Deleting page data: {page.DisplayName}");
            foreach (Field field in page.Fields)
            {
                if (field is GridField gridField)
                {
                    @this.DeleteDataTableForGrid(page.GetView(), gridField);
                }
            }
            if (@this.TableExists(page.TableName))
            {
                @this.DeleteTable(page.TableName);
            }
        }

        public static void DeleteView(this CollectedDataProvider @this, View view)
        {
            Log.Instance.Debug($"Deleting view data: {view.DisplayName}");
            foreach (Page page in view.Pages)
            {
                @this.DeletePage(page);
            }
            if (@this.TableExists(view.TableName))
            {
                @this.DeleteTable(view.TableName);
            }
        }
    }
}
