using Epi;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.EpiInfo.Metadata;
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

        public static int GetMaxPagePosition(this View @this)
        {
            return @this.Pages.Select(page => page.Position)
                .DefaultIfEmpty(-1)
                .Max();
        }

        public static FieldDataTable GetFields(this View @this)
        {
            CollectedDataProvider collectedData = @this.Project.CollectedData;
            IDbDriver driver = collectedData.GetDbDriver();

            string Quote(string identifier)
            {
                return driver.InsertInEscape(identifier);
            }

            string sql = $@"
                SELECT
                    F.*,
                    P.{Quote(ColumnNames.POSITION)}
                FROM metaFields AS F
                LEFT OUTER JOIN metaPages AS P ON P.{Quote(ColumnNames.PAGE_ID)} = F.{Quote(ColumnNames.PAGE_ID)}
                WHERE F.{Quote(ColumnNames.VIEW_ID)} = @ViewId;";
            Query query = collectedData.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, @this.Id));
            return new FieldDataTable(collectedData.Select(query));
        }

        public static void LoadFields(this View @this)
        {
            @this.MustRefreshFieldCollection = true;
            _ = @this.Fields;
        }

        public static bool TableExists(this View @this)
        {
            return @this.Project.CollectedData.TableExists(@this.TableName);
        }

        public static void Synchronize(this View @this)
        {
            Log.Instance.Debug($"Synchronizing view: {@this.DisplayName}");
            @this.Project.CollectedData.SynchronizeDataTable(@this);
        }

        public static void SynchronizeTree(this View @this)
        {
            @this.Synchronize();
            foreach (View descendantView in @this.GetDescendantViews())
            {
                descendantView.Synchronize();
            }
        }

        public static void DeleteData(this View @this)
        {
            Log.Instance.Debug($"Deleting view data: {@this.DisplayName}");
            foreach (Page page in @this.Pages)
            {
                page.DeleteData();
            }
            CollectedDataProvider collectedData = @this.Project.CollectedData;
            if (collectedData.TableExists(@this.TableName))
            {
                collectedData.DeleteTable(@this.TableName);
            }
        }

        public static void DeleteMetadata(this View @this)
        {
            Log.Instance.Debug($"Deleting view metadata: {@this.DisplayName}");
            foreach (Page page in @this.Pages)
            {
                page.DeleteMetadata();
            }
            @this.GetMetadata().DeleteView(@this.Name);
        }

        public static void Delete(this View @this, Page page)
        {
            Log.Instance.Debug($"Deleting page: {page.DisplayName}");
            page.DeleteData();
            page.DeleteMetadata();
            @this.Pages.Remove(page);
            @this.GetMetadata().SynchronizePageNumbersOnDelete(@this, page.Position);
            @this.MustRefreshFieldCollection = true;
        }

        public static void Unrelate(this View @this)
        {
            Log.Instance.Debug($"Unrelating view: {@this.DisplayName}");
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
