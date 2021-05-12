using Epi;
using Epi.Data.Services;
using ERHMS.Common.Logging;

namespace ERHMS.EpiInfo.Metadata
{
    public static class IMetadataProviderExtensions
    {
        public static void DeepDeletePage(this IMetadataProvider @this, Page page)
        {
            Log.Instance.Debug($"Deleting page metadata: {page.DisplayName}");
            @this.DeleteFields(page);
            @this.DeletePage(page);
        }

        public static void DeepDeleteView(this IMetadataProvider @this, View view)
        {
            Log.Instance.Debug($"Deleting view metadata: {view.DisplayName}");
            foreach (Page page in view.Pages)
            {
                @this.DeepDeletePage(page);
            }
            @this.DeleteView(view.Name);
        }
    }
}
