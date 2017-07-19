using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public static class ObjectExtensions
    {
        private const int OffsetBasis = unchecked((int)2166136261);
        private const int Prime = 16777619;

        // http://www.isthe.com/chongo/tech/comp/fnv/#FNV-1a
        public static int GetHashCode(IEnumerable<object> values)
        {
            int hash = OffsetBasis;
            foreach (object value in values)
            {
                hash = (hash ^ (value?.GetHashCode() ?? 0)) * Prime;
            }
            return hash;
        }

        public static int GetHashCode<T>(T @this, IEnumerable<Func<T, object>> identifiers)
        {
            return GetHashCode(identifiers.Select(identifier => identifier(@this)));
        }

        public static bool Equals<T>(T @this, object obj, IEnumerable<Func<T, object>> identifiers) where T : class
        {
            T t = obj as T;
            return t != null && identifiers.All(identifier => Equals(identifier(t), identifier(@this)));
        }
    }
}
