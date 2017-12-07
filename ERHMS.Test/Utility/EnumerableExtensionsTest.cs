using ERHMS.Test.Infrastructure;
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
            AssertExtensions.AreEqual(EnumerableExtensions.Yield(1), 1);
        }

        [Test]
        public void IterateTest()
        {
            ICollection<Iterator<int>> numbers = Enumerable.Range(1, 10).Iterate().ToList();
            CollectionAssert.AreEqual(Enumerable.Range(0, 10), numbers.Select(number => number.Index));
            CollectionAssert.AreEqual(Enumerable.Range(1, 10), numbers.Select(number => number.Value));
        }

        [Test]
        public void ElementsAtTest()
        {
            ICollection<int> numbers = Enumerable.Range(1, 10).ToList();
            AssertExtensions.AreEqual(numbers.ElementsAt(0, 1, 2), 1, 2, 3);
            AssertExtensions.AreEqual(numbers.ElementsAt(9, 8, 7), 10, 9, 8);
            CollectionAssert.IsEmpty(numbers.ElementsAt());
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
