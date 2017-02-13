using System.Collections.Generic;

namespace ERHMS.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> @this, T item)
        {
            yield return item;
            foreach (T t in @this)
            {
                yield return t;
            }
        }
    }
}
