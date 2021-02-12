using Epi;
using Epi.Data.Services;
using Epi.Fields;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static void CreateDataTables(this View @this)
        {
            if (!@this.Project.CollectedData.TableExists(@this.TableName))
            {
                @this.Project.CollectedData.CreateDataTableForView(@this, 1);
            }
        }

        public static void CreateDataTableTree(this View @this)
        {
            CreateDataTables(@this);
            foreach (View descendantView in @this.GetDescendantViews())
            {
                CreateDataTables(descendantView);
            }
        }

        public static void DeleteDataTableTree(this View @this)
        {
            @this.DeleteDataTables();
            foreach (View descendantView in @this.GetDescendantViews())
            {
                descendantView.DeleteDataTables();
            }
        }

        public static void DeleteMetadata(this View @this)
        {
            IMetadataProvider metadata = @this.GetMetadata();
            foreach (Page page in @this.Pages)
            {
                metadata.DeleteFields(page);
                metadata.DeletePage(page);
            }
            metadata.DeleteView(@this.Name);
        }

        public static void Unrelate(this View @this)
        {
            if (!@this.IsRelatedView)
            {
                return;
            }
            IMetadataProvider metadata = @this.GetMetadata();
            if (@this.ParentView != null)
            {
                IReadOnlyCollection<Field> relatedViewFields = @this.ParentView.Fields
                    .RelatedFields
                    .Cast<RelatedViewField>()
                    .Where(field => field.RelatedViewID == @this.Id)
                    .ToList();
                foreach (Field relatedViewField in relatedViewFields)
                {
                    metadata.DeleteField(relatedViewField);
                    @this.ParentView.Fields.Remove(relatedViewField);
                }
            }
            @this.IsRelatedView = false;
            metadata.UpdateView(@this);
        }
    }
}
