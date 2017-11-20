using ERHMS.Test.Infrastructure;
using ERHMS.Utility;
using NUnit.Framework;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class ArrayExtensionsTest
    {
        [Test]
        public void ResizeTest()
        {
            int[] numbers = Enumerable.Range(1, 5).ToArray();
            ArrayExtensions.Resize(ref numbers, 6, 0);
            AssertExtensions.AreEqual(numbers, 1, 2, 3, 4, 5, 0);
            ArrayExtensions.Resize(ref numbers, 7, 1);
            AssertExtensions.AreEqual(numbers, 0, 1, 2, 3, 4, 5, 0);
            ArrayExtensions.Resize(ref numbers, 6, 0);
            AssertExtensions.AreEqual(numbers, 0, 1, 2, 3, 4, 5);
            ArrayExtensions.Resize(ref numbers, 5, 1);
            AssertExtensions.AreEqual(numbers, 0, 0, 1, 2, 3);
        }
    }
}
