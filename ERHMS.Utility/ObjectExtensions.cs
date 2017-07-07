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
        public static int GetHashCode<T>(T @this, IEnumerable<Func<T, object>> identifiers)
        {
            int hash = OffsetBasis;
            foreach (Func<T, object> identifier in identifiers)
            {
                hash = (hash ^ (identifier(@this)?.GetHashCode()).GetValueOrDefault()) * Prime;
            }
            return hash;
        }

        public static bool Equals<T>(T @this, object obj, IEnumerable<Func<T, object>> identifiers) where T : class
        {
            T t = obj as T;
            return t != null && identifiers.All(identifier => Equals(identifier(t), identifier(@this)));
        }
    }
}
