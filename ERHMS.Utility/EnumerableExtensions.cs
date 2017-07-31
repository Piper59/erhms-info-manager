using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(T item)
        {
            yield return item;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T item)
        {
            return @this.Concat(Yield(item));
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T item)
        {
            return Yield(item).Concat(@this);
        }

        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                @this.Add(item);
            }
        }
    }
}
