using Epi;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static bool DataTableExists(this View @this)
        {
            return @this.Project.CollectedData.TableExists(@this.TableName);
        }

        public static void DeleteDataTablesEx(this View @this)
        {
            Log.Default.Debug($"Deleting data tables: {@this.DisplayName}");
            @this.DeleteDataTables();
        }

        public static void DeleteDataTableTree(this View @this)
        {
            Log.Default.Debug($"Deleting data table tree: {@this.DisplayName}");
            foreach (View descendantView in @this.GetDescendantViews())
            {
                descendantView.DeleteDataTablesEx();
            }
            @this.DeleteDataTablesEx();
        }

        public static void DeleteMetadata(this View @this)
        {
            Log.Default.Debug($"Deleting metadata: {@this.DisplayName}");
            IMetadataProvider metadata = @this.GetMetadata();
            foreach (Page page in @this.Pages)
            {
                metadata.DeleteFields(page);
                metadata.DeletePage(page);
            }
            metadata.DeleteView(@this.Name);
        }

        public static void DeletePageEx(this View @this, Page page)
        {
            Log.Default.Debug($"Deleting page: {page.DisplayName}");
            page.DeleteDataTables();
            page.DeleteMetadata();
            @this.Pages.Remove(page);
            @this.MustRefreshFieldCollection = true;
        }

        public static void LoadFields(this View @this)
        {
            @this.MustRefreshFieldCollection = true;
            _ = @this.Fields;
        }

        public static void SynchronizeDataTables(this View @this)
        {
            Log.Default.Debug($"Synchronizing data tables: {@this.DisplayName}");
            @this.Project.CollectedData.SynchronizeDataTable(@this);
        }

        public static void SynchronizeDataTableTree(this View @this)
        {
            Log.Default.Debug($"Synchronizing data table tree: {@this.DisplayName}");
            foreach (View descendantView in @this.GetDescendantViews())
            {
                descendantView.SynchronizeDataTables();
            }
            @this.SynchronizeDataTables();
        }

        public static void Unrelate(this View @this)
        {
            Log.Default.Debug($"Unrelating view: {@this.DisplayName}");
            if (!@this.IsRelatedView)
            {
                return;
            }
            IMetadataProvider metadata = @this.GetMetadata();
            if (@this.ParentView != null)
            {
                IReadOnlyCollection<Field> relatedViewFields = @this.ParentView.Fields.RelatedFields
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
