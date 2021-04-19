using Epi;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Metadata
{
    public static class FieldDataRowComparer
    {
        private class DefaultImpl : IComparer<FieldDataRow>
        {
            public int Compare(FieldDataRow field1, FieldDataRow field2)
            {
                return Comparer<int>.Default.Compare(field1.FieldId, field2.FieldId);
            }
        }

        public class ByTabOrder : IComparer<FieldDataRow>
        {
            public int Compare(FieldDataRow field1, FieldDataRow field2)
            {
                int result = Comparer<short?>.Default.Compare(field1.Position, field2.Position);
                if (result == 0)
                {
                    result = Comparer<double?>.Default.Compare(field1.TabIndex, field2.TabIndex);
                }
                if (result == 0)
                {
                    result = Default.Compare(field1, field2);
                }
                return result;
            }
        }

        public class ByGroupHoistingTabOrder : IComparer<FieldDataRow>
        {
            private readonly IReadOnlyDictionary<string, double?> tabIndicesByFieldName;

            public ByGroupHoistingTabOrder(IEnumerable<FieldDataRow> fields)
            {
                tabIndicesByFieldName = fields.ToDictionary(field => field.Name, field => field.TabIndex);
            }

            private double? GetTabOrder(FieldDataRow field)
            {
                if (field.FieldType == MetaFieldType.Group && field.List != null)
                {
                    double? minChildTabIndex = null;
                    foreach (string childFieldName in field.ListItems)
                    {
                        if (!tabIndicesByFieldName.TryGetValue(childFieldName, out double? childTabIndex))
                        {
                            continue;
                        }
                        if (minChildTabIndex == null || childTabIndex < minChildTabIndex)
                        {
                            minChildTabIndex = childTabIndex;
                        }
                    }
                    return minChildTabIndex == null ? field.TabIndex : minChildTabIndex - 0.5;
                }
                else
                {
                    return field.TabIndex;
                }
            }

            public int Compare(FieldDataRow field1, FieldDataRow field2)
            {
                int result = Comparer<short?>.Default.Compare(field1.Position, field2.Position);
                if (result == 0)
                {
                    result = Comparer<double?>.Default.Compare(GetTabOrder(field1), GetTabOrder(field2));
                }
                if (result == 0)
                {
                    result = Default.Compare(field1, field2);
                }
                return result;
            }
        }

        public static IComparer<FieldDataRow> Default { get; } = new DefaultImpl();
    }
}
