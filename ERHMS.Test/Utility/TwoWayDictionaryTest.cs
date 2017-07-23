using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class TwoWayDictionaryTest
    {
        private TwoWayDictionary<int, char> numbers;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            numbers = new TwoWayDictionary<int, char>
            {
                { 1, 'A' },
                { 2, 'B' },
                { 3, 'C' },
                { 4, 'D' },
                { 5, 'E' }
            };
        }

        [Test]
        public void AddAndRemoveTest()
        {
            Assert.AreEqual(5, numbers.Count);
            Tuple<int, char> six = Tuple.Create(6, 'F');
            numbers.Add(six);
            Assert.Catch(() =>
            {
                numbers.Add(six);
            });
            Assert.Catch(() =>
            {
                numbers.Add(Tuple.Create(6, ' '));
            });
            Assert.Catch(() =>
            {
                numbers.Add(Tuple.Create(0, 'F'));
            });
            Assert.AreEqual(6, numbers.Count);
            Assert.IsTrue(numbers.Contains(six));
            Assert.IsTrue(numbers.Remove(six));
            Assert.IsFalse(numbers.Remove(six));
            Assert.AreEqual(5, numbers.Count);
            Assert.IsFalse(numbers.Contains(six));
        }

        [Test]
        public void ForwardAndReverseTest()
        {
            Assert.AreEqual('A', numbers.Forward(1));
            Assert.Catch(() =>
            {
                numbers.Forward(6);
            });
            Assert.AreEqual(1, numbers.Reverse('A'));
            Assert.Catch(() =>
            {
                numbers.Reverse('F');
            });
        }
    }
}
