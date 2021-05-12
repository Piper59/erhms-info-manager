using Epi;
using ERHMS.EpiInfo.Naming;
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

        public class ByTabIndex : IComparer<FieldDataRow>
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

        public class ByEffectiveTabIndex : IComparer<FieldDataRow>
        {
            private readonly IReadOnlyDictionary<string, double?> tabIndicesByFieldName;

            public ByEffectiveTabIndex(IEnumerable<FieldDataRow> fields)
            {
                tabIndicesByFieldName = fields.ToDictionary(
                    field => field.Name,
                    field => field.TabIndex,
                    NameComparer.Default);
            }

            private IEnumerable<double?> GetChildTabIndices(FieldDataRow groupField)
            {
                if (groupField.List == null)
                {
                    yield break;
                }
                foreach (string childFieldName in groupField.ListItems)
                {
                    if (tabIndicesByFieldName.TryGetValue(childFieldName, out double? childTabIndex))
                    {
                        yield return childTabIndex;
                    }
                }
            }

            private double? GetEffectiveTabIndex(FieldDataRow field)
            {
                if (field.FieldType == MetaFieldType.Group)
                {
                    double? minChildTabIndex = GetChildTabIndices(field).Min();
                    if (minChildTabIndex != null)
                    {
                        return minChildTabIndex - 0.5;
                    }
                }
                return field.TabIndex;
            }

            public int Compare(FieldDataRow field1, FieldDataRow field2)
            {
                int result = Comparer<short?>.Default.Compare(field1.Position, field2.Position);
                if (result == 0)
                {
                    result = Comparer<double?>.Default.Compare(
                        GetEffectiveTabIndex(field1),
                        GetEffectiveTabIndex(field2));
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
