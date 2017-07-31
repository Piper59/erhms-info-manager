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
        public void AddRangeTest()
        {
            ICollection<int> numbers = new List<int>();
            numbers.AddRange(Enumerable.Range(1, 10));
            CollectionAssert.AreEqual(Enumerable.Range(1, 10), numbers);
            numbers.AddRange(Enumerable.Range(11, 10));
            CollectionAssert.AreEqual(Enumerable.Range(1, 20), numbers);
        }
    }
}
