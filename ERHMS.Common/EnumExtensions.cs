using System;
using System.Linq;

namespace ERHMS.Common
{
    public static class EnumExtensions
    {
        public static bool HasAnyFlag(this Enum @this, params Enum[] flags)
        {
            return flags.Any(@this.HasFlag);
        }

        public static bool HasAllFlags(this Enum @this, params Enum[] flags)
        {
            return flags.All(@this.HasFlag);
        }
    }
}
