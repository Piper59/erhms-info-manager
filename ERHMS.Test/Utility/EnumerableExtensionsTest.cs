using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class EnumerableExtensionsTest
    {
        [Test]
        public void YieldTest()
        {
            CollectionAssert.AreEqual(new int[] { 1 }, EnumerableExtensions.Yield(1));
        }

        [Test]
        public void AppendTest()
        {
            CollectionAssert.AreEqual(Enumerable.Range(1, 11), Enumerable.Range(1, 10).Append(11));
        }

        [Test]
        public void PrependTest()
        {
            CollectionAssert.AreEqual(Enumerable.Range(0, 11), Enumerable.Range(1, 10).Prepend(0));
        }

        [Test]
        public void AsReadOnlyTest()
        {
            ICollection<int> numbers = Enumerable.Range(1, 10).ToArray().AsReadOnly();
            CollectionAssert.AreEqual(Enumerable.Range(1, 10), numbers);
            Assert.IsTrue(numbers.IsReadOnly);
            Assert.Catch(() =>
            {
                numbers.Add(11);
            });
        }
    }
}
