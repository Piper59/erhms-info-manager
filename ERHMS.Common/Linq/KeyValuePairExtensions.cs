using System.Collections.Generic;

namespace ERHMS.Common.Linq
{
    public static class KeyValuePairExtensions
    {
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> @this,
            out TKey key,
            out TValue value)
        {
            key = @this.Key;
            value = @this.Value;
        }
    }
}
