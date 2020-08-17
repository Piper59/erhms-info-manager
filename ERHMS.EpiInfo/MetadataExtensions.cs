using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public static class MetadataExtensions
    {
        private const double GroupTabIndexDifference = 0.1;

        public static IEnumerable<DataRow> GetSortedFields(this IMetadataProvider @this, int pageId)
        {
            DataTable fields = @this.GetFieldsOnPageAsDataTable(pageId);
            fields.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            IDictionary<string, DataRow> fieldsByName = fields.AsEnumerable().ToDictionary(
                field => field.Field<string>(ColumnNames.NAME),
                field => field,
                StringComparer.OrdinalIgnoreCase);
            foreach (DataRow field in fields.Rows)
            {
                if (field.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID) == MetaFieldType.Group)
                {
                    double? minTabIndex = null;
                    string childFieldNames = field.Field<string>(ColumnNames.LIST);
                    foreach (string childFieldName in childFieldNames.Split(Constants.LIST_SEPARATOR))
                    {
                        if (fieldsByName.TryGetValue(childFieldName, out DataRow childField))
                        {
                            double? tabIndex = childField.Field<double?>(ColumnNames.TAB_INDEX);
                            if (tabIndex != null && (minTabIndex == null || tabIndex < minTabIndex))
                            {
                                minTabIndex = tabIndex;
                            }
                        }
                    }
                    if (minTabIndex != null)
                    {
                        field.SetField(ColumnNames.TAB_INDEX, minTabIndex.Value - GroupTabIndexDifference);
                    }
                }
            }
            return fields.Select(null, ColumnNames.TAB_INDEX);
        }

        public static IEnumerable<string> GetSortedFieldNames(this IMetadataProvider @this, int viewId, Predicate<MetaFieldType> predicate = null)
        {
            ICollection<string> columnNames = new string[]
            {
                $"F.[{ColumnNames.NAME}]",
                $"F.[{ColumnNames.FIELD_ID}]",
                $"F.[{ColumnNames.FIELD_TYPE_ID}]",
                $"F.[{ColumnNames.TAB_INDEX}]",
                $"P.[{ColumnNames.POSITION}]"
            };
            string sql = $@"
                SELECT {string.Join(", ", columnNames)}
                FROM [metaFields] AS F
                LEFT OUTER JOIN [metaPages] AS P ON F.[{ColumnNames.PAGE_ID}] = P.[{ColumnNames.PAGE_ID}]
                WHERE F.[{ColumnNames.VIEW_ID}] = @ViewId;";
            Query query = @this.Project.CollectedData.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            DataTable table = @this.Project.CollectedData.Select(query);
            table.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            table.SetColumnDataType(ColumnNames.POSITION, typeof(int));
            IEnumerable<DataRow> fields = table.AsEnumerable();
            if (predicate != null)
            {
                fields = fields.Where(field => predicate(field.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID)));
            }
            return fields.OrderBy(field => field.IsNull(ColumnNames.POSITION) ? -1 : field.Field<int>(ColumnNames.POSITION))
                .ThenBy(field => field.IsNull(ColumnNames.TAB_INDEX) ? -1.0 : field.Field<double>(ColumnNames.TAB_INDEX))
                .ThenBy(field => field.Field<int>(ColumnNames.FIELD_ID))
                .Select(field => field.Field<string>(ColumnNames.NAME));
        }
    }
}
