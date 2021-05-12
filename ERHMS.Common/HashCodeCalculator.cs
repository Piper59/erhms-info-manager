namespace ERHMS.Common
{
    // https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV-1a_hash
    public static class HashCodeCalculator
    {
        private const int OffsetBasis = unchecked((int)2166136261);
        private const int Prime = 16777619;

        public static int Combine(int hashCode, object obj)
        {
            if (obj != null)
            {
                hashCode ^= obj.GetHashCode();
            }
            return hashCode * Prime;
        }

        public static int GetHashCode(object obj1, object obj2)
        {
            int hashCode = OffsetBasis;
            hashCode = Combine(hashCode, obj1);
            hashCode = Combine(hashCode, obj2);
            return hashCode;
        }

        public static int GetHashCode(object obj1, object obj2, object obj3)
        {
            int hashCode = OffsetBasis;
            hashCode = Combine(hashCode, obj1);
            hashCode = Combine(hashCode, obj2);
            hashCode = Combine(hashCode, obj3);
            return hashCode;
        }

        public static int GetHashCode(object obj1, object obj2, object obj3, object obj4)
        {
            int hashCode = OffsetBasis;
            hashCode = Combine(hashCode, obj1);
            hashCode = Combine(hashCode, obj2);
            hashCode = Combine(hashCode, obj3);
            hashCode = Combine(hashCode, obj4);
            return hashCode;
        }

        public static int GetHashCode(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            int hashCode = OffsetBasis;
            hashCode = Combine(hashCode, obj1);
            hashCode = Combine(hashCode, obj2);
            hashCode = Combine(hashCode, obj3);
            hashCode = Combine(hashCode, obj4);
            hashCode = Combine(hashCode, obj5);
            return hashCode;
        }
    }
}
