using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common.Linq
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TItem> Yield<TItem>(params TItem[] items)
        {
            foreach (TItem item in items)
            {
                yield return item;
            }
        }

        public static IEnumerable<TItem> Append<TItem>(this IEnumerable<TItem> @this, params TItem[] items)
        {
            return Enumerable.Concat(@this, items);
        }

        public static IEnumerable<TItem> Prepend<TItem>(this IEnumerable<TItem> @this, params TItem[] items)
        {
            return Enumerable.Concat(items, @this);
        }

        public static IEnumerable<Iterator<TItem>> Iterate<TItem>(this IEnumerable<TItem> @this)
        {
            return @this.Select((value, index) => Iterator.Create(index, value));
        }
    }
}
