using System.Collections.Generic;

namespace ERHMS.Common
{
    public static class HashCodeCalculator
    {
        private const int OffsetBasis = unchecked((int)2166136261);
        private const int Prime = 16777619;

        private static int Combine(int hashCode1, int hashCode2)
        {
            return (hashCode1 ^ hashCode2) * Prime;
        }

        private static int GetHashCodeCore(object obj)
        {
            return EqualityComparer<object>.Default.GetHashCode(obj);
        }

        public static int GetHashCode(object obj1)
        {
            return Combine(OffsetBasis, GetHashCodeCore(obj1));
        }

        public static int GetHashCode(object obj1, object obj2)
        {
            return Combine(GetHashCode(obj1), GetHashCodeCore(obj2));
        }

        public static int GetHashCode(object obj1, object obj2, object obj3)
        {
            return Combine(GetHashCode(obj1, obj2), GetHashCodeCore(obj3));
        }

        public static int GetHashCode(object obj1, object obj2, object obj3, object obj4)
        {
            return Combine(GetHashCode(obj1, obj2, obj3), GetHashCodeCore(obj4));
        }

        public static int GetHashCode(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return Combine(GetHashCode(obj1, obj2, obj3, obj4), GetHashCodeCore(obj5));
        }
    }
}
