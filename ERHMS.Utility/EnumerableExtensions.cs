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
            foreach (T t in @this)
            {
                yield return t;
            }
            yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T item)
        {
            yield return item;
            foreach (T t in @this)
            {
                yield return t;
            }
        }

        public static IEnumerable<Iterator<T>> Iterate<T>(this IEnumerable<T> @this)
        {
            return @this.Select((value, index) => new Iterator<T>(value, index));
        }
    }
}
