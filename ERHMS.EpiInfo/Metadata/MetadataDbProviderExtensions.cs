using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.EpiInfo.Data;
using System.Data;

namespace ERHMS.EpiInfo.Metadata
{
    public static class MetadataDbProviderExtensions
    {
        public static FieldDataTable GetFieldDataTableForPage(this MetadataDbProvider @this, int pageId)
        {
            return new FieldDataTable(@this.GetFieldsOnPageAsDataTable(pageId));
        }

        public static FieldDataTable GetFieldDataTableForView(this MetadataDbProvider @this, int viewId)
        {
            string sql = $@"
                SELECT
                    F.*,
                    P.{@this.db.Quote(ColumnNames.POSITION)}
                FROM metaFields AS F
                LEFT OUTER JOIN metaPages AS P ON
                    P.{@this.db.Quote(ColumnNames.PAGE_ID)} = F.{@this.db.Quote(ColumnNames.PAGE_ID)}
                WHERE F.{@this.db.Quote(ColumnNames.VIEW_ID)} = @ViewId;";
            Query query = @this.db.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            return new FieldDataTable(@this.db.Select(query));
        }

        public static GridColumnDataTable GetGridColumnDataTable(this MetadataDbProvider @this, int fieldId)
        {
            string sql = $@"
                SELECT *
                FROM metaGridColumns
                WHERE {@this.db.Quote(ColumnNames.FIELD_ID)} = @FieldId";
            Query query = @this.db.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@FieldId", DbType.Int32, fieldId));
            return new GridColumnDataTable(@this.db.Select(query));
        }
    }
}
