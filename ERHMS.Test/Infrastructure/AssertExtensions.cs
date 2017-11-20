using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test.Infrastructure
{
    public static class AssertExtensions
    {
        public static void AreEqual<T>(IEnumerable<T> actual, params T[] expected)
        {
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
