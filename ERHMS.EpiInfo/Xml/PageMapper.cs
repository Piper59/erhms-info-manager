using Epi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class PageMapper : Mapper<Page>
    {
        private const double GroupTabIndexDifference = 0.2;
        private static readonly ISet<string> IgnoredFieldAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UniqueId",
            "Expr1015",
            "Expr1016",
            "Expr1017"
        };

        protected override string ElementName => ElementNames.Page;

        public PageMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(p => p.Id, attributeName: ColumnNames.PAGE_ID),
                Mapping.FromExpr(p => p.Name),
                Mapping.FromExpr(p => p.Position),
                Mapping.FromExpr(p => p.BackgroundId),
                Mapping.FromFunc(p => p.GetView().Id, Mapping.Ignored, ColumnNames.VIEW_ID)
            };
        }

        private IEnumerable<DataRow> GetFieldsAsDataRows(Page model)
        {
            IDictionary<string, DataRow> rows = model.GetMetadata()
                .GetFieldsOnPageAsDataTable(model.Id)
                .Rows
                .Cast<DataRow>()
                .ToDictionary(row => row.Field<string>(ColumnNames.NAME), StringComparer.OrdinalIgnoreCase);
            IEnumerable<DataRow> groups = rows.Values.Where(row => (MetaFieldType)row.Field<int>(ColumnNames.FIELD_TYPE_ID) == MetaFieldType.Group);
            foreach (DataRow group in groups)
            {
                IEnumerable<string> memberNames = group.Field<string>(ColumnNames.LIST).Split(Constants.LIST_SEPARATOR);
                double minTabIndex = memberNames.Min(memberName => rows[memberName].Field<double>(ColumnNames.TAB_INDEX));
                group.SetField(ColumnNames.TAB_INDEX, minTabIndex - GroupTabIndexDifference);
            }
            return rows.Values.OrderBy(field => field.Field<double>(ColumnNames.TAB_INDEX));
        }

        public IEnumerable<XElement> GetFieldElements(Page model)
        {
            ICollection<DataRow> rows = GetFieldsAsDataRows(model).ToList();
            foreach (DataRow row in rows)
            {
                XElement element = new XElement(ElementNames.Field);
                MetaFieldType fieldType = (MetaFieldType)row.Field<int>(ColumnNames.FIELD_TYPE_ID);
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (!ConfigurationExtensions.CompatibilityMode && IgnoredFieldAttributes.Contains(column.ColumnName))
                    {
                        continue;
                    }
                    if (column.ColumnName == ColumnNames.BACKGROUND_COLOR && string.IsNullOrEmpty(row.Field<string>(column)))
                    {
                        continue;
                    }
                    element.Add(new XAttribute(column.ColumnName, row[column]));
                    if (column.ColumnName == "RelatedViewId" && fieldType == MetaFieldType.Relate)
                    {
                        int? relatedViewId = row.Field<int?>(column);
                        if (relatedViewId != null)
                        {
                            View view = model.GetMetadata().GetViewById(relatedViewId.Value);
                            if (view != null)
                            {
                                element.Add(new XAttribute("RelatedViewName", view.Name));
                            }
                        }
                    }
                }
                yield return element;
            }
        }
    }
}
