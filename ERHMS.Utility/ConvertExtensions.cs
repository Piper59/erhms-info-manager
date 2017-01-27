using System;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static Guid? ToNullableGuid(string value)
        {
            Guid result;
            return Guid.TryParse(value, out result) ? result : (Guid?)null;
        }
    }
}
