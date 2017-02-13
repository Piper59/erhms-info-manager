using ERHMS.Utility;
using NUnit.Framework;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void PrependTest()
        {
            CollectionAssert.AreEqual(Enumerable.Range(0, 11), Enumerable.Range(1, 10).Prepend(0));
        }
    }
}
