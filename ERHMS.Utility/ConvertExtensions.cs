using System;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static Guid? ToNullableGuid(string value)
        {
            return value == null ? (Guid?)null : Guid.Parse(value);
        }
    }
}
