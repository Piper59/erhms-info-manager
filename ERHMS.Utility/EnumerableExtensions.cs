using System.Collections.Generic;

namespace ERHMS.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T item)
        {
            foreach (T existingItem in @this)
            {
                yield return existingItem;
            }
            yield return item;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T item)
        {
            yield return item;
            foreach (T existingItem in @this)
            {
                yield return existingItem;
            }
        }
    }
}
