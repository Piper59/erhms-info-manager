using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common.Linq
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TItem> Yield<TItem>(TItem item)
        {
            yield return item;
        }

        public static IEnumerable<TItem> Append<TItem>(this IEnumerable<TItem> @this, TItem item)
        {
            return @this.Concat(Yield(item));
        }

        public static IEnumerable<TItem> Prepend<TItem>(this IEnumerable<TItem> @this, TItem item)
        {
            return Yield(item).Concat(@this);
        }

        public static IEnumerable<Iterator<TItem>> Iterate<TItem>(this IEnumerable<TItem> @this)
        {
            return @this.Select((value, index) => new Iterator<TItem>(index, value));
        }
    }
}
