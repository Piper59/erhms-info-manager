using Epi.Fields;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Metadata
{
    public static class FieldComparer
    {
        private class DefaultImpl : IComparer<Field>
        {
            public int Compare(Field field1, Field field2)
            {
                return Comparer<int>.Default.Compare(field1.Id, field2.Id);
            }
        }

        public class ByTabIndex : IComparer<Field>
        {
            private int Compare(RenderableField field1, RenderableField field2)
            {
                int result = Comparer<int>.Default.Compare(field1.Page.Position, field2.Page.Position);
                if (result == 0)
                {
                    result = Comparer<double>.Default.Compare(field1.TabIndex, field2.TabIndex);
                    if (result == 0)
                    {
                        result = Default.Compare(field1, field2);
                    }
                }
                return result;
            }

            public int Compare(Field field1, Field field2)
            {
                if (field1 is RenderableField renderableField1 && field2 is RenderableField renderableField2)
                {
                    return Compare(renderableField1, renderableField2);
                }
                else
                {
                    return Default.Compare(field1, field2);
                }
            }
        }

        public static IComparer<Field> Default { get; } = new DefaultImpl();
    }
}
