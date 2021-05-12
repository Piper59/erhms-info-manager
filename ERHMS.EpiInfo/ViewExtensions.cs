using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Naming;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static Page GetPageByName(this View @this, string pageName)
        {
            return @this.Pages.SingleOrDefault(page => NameComparer.Default.Equals(page.Name, pageName));
        }

        public static void Unrelate(this View @this)
        {
            Log.Instance.Debug($"Unrelating view: {@this.DisplayName}");
            IMetadataProvider metadata = @this.GetMetadata();
            if (@this.ParentView != null)
            {
                IEnumerable<Field> relateFields = @this.ParentView.Fields.RelatedFields
                    .Cast<RelatedViewField>()
                    .Where(field => field.RelatedViewID == @this.Id);
                foreach (Field relateField in relateFields.ToList())
                {
                    metadata.DeleteField(relateField);
                    @this.ParentView.Fields.Remove(relateField);
                }
            }
            @this.IsRelatedView = false;
            metadata.UpdateView(@this);
        }
    }
}
