using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common.Linq
{
    public static class EnumerableExtensions
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
            return @this.Select((value, index) => new Iterator<TItem>(value, index));
        }

        public static void AddRange<TItem>(this ICollection<TItem> @this, IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                @this.Add(item);
            }
        }
    }
}
