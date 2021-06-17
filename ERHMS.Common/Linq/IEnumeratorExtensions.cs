using System;
using System.Collections.Generic;

namespace ERHMS.Common.Linq
{
    public static class IEnumeratorExtensions
    {
        public static TItem GetNext<TItem>(this IEnumerator<TItem> @this)
        {
            if (!@this.MoveNext())
            {
                throw new InvalidOperationException("Cannot advance enumerator.");
            }
            return @this.Current;
        }

        public static TItem GetNextOrDefault<TItem>(this IEnumerator<TItem> @this, TItem defaultValue = default)
        {
            return @this.MoveNext() ? @this.Current : defaultValue;
        }
    }
}
