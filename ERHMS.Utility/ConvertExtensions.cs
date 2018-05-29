using System;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static Guid? ToNullableGuid(string value)
        {
            return string.IsNullOrEmpty(value) ? (Guid?)null : Guid.Parse(value);
        }
    }
}
