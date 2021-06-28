using System;

namespace ERHMS.Common
{
    public static class MathExtensions
    {
        public static TValue Clamp<TValue>(this TValue @this, TValue min, TValue max)
            where TValue : IComparable<TValue>
        {
            if (@this.CompareTo(min) < 0)
            {
                return min;
            }
            if (@this.CompareTo(max) > 0)
            {
                return max;
            }
            return @this;
        }
    }
}
