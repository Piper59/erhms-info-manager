using System;

namespace ERHMS.Common.Text
{
    public static class Comparers
    {
        public static StringComparer Arg => StringComparer.OrdinalIgnoreCase;
        public static StringComparer Path => StringComparer.OrdinalIgnoreCase;
    }
}
