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

        public class EffectiveTabIndexAware : IComparer<FieldDataRow>
        {
            private readonly IReadOnlyDictionary<string, double?> tabIndicesByFieldName;

            public EffectiveTabIndexAware(IEnumerable<FieldDataRow> fields)
            {
                tabIndicesByFieldName = fields.ToDictionary(field => field.Name, field => field.TabIndex);
            }

            public double? GetEffectiveTabIndex(FieldDataRow field)
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
                int result = Comparer<double?>.Default.Compare(
                    GetEffectiveTabIndex(field1),
                    GetEffectiveTabIndex(field2));
                return result == 0 ? Default.Compare(field1, field2) : result;
            }
        }

        public static IComparer<FieldDataRow> Default { get; } = new DefaultImpl();
    }
}
