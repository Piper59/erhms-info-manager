using Epi;
using Epi.Collections;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static void LoadFields(this View @this)
        {
            @this.MustRefreshFieldCollection = true;
            FieldCollectionMaster fields = @this.Fields;
            return;
        }
    }
}
