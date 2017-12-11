using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test
{
    public static class AssertExtensions
    {
        public static void AreEqual<T>(IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEqual(expected, actual);
        }

        public static void AreEquivalent<T>(IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
