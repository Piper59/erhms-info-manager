using Epi;
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
            IDictionary<string, DataRow> fieldsByName = fields.Rows.Cast<DataRow>().ToDictionary(
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
    }
}
