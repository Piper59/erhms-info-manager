using System;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static Guid? ToNullableGuid(string value)
        {
            Guid guid;
            return Guid.TryParse(value, out guid) ? guid : (Guid?)null;
        }
    }
}
