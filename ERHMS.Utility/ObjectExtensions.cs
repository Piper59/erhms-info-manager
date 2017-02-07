namespace ERHMS.Utility
{
    public static class ObjectExtensions
    {
        // http://www.isthe.com/chongo/tech/comp/fnv/
        public static int GetHashCode(params object[] values)
        {
            unchecked
            {
                int hashCode = (int)2166136261;
                foreach (object value in values)
                {
                    hashCode = (hashCode * 16777619) ^ (value == null ? 0 : value.GetHashCode());
                }
                return hashCode;
            }
        }
    }
}
