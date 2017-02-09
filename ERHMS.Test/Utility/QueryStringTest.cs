using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ERHMS.Test.Utility
{
    public class QueryStringTest
    {
        private QueryString query;

        [SetUp]
        public void SetUp()
        {
            query = new QueryString();
            query.Set("key1", "value1");
            query.Set("key2", "value2");
            query.Set("key3", "value3");
        }

        [Test]
        public void ParseTest()
        {
            query = QueryString.Parse("zero=&one=1&two=2&three=3");
            Assert.AreEqual(4, query.Count);
            Assert.AreEqual("", query.Get("zero"));
            Assert.AreEqual("1", query.Get("one"));
            Assert.IsNull(query.Get("four"));
        }

        [Test]
        public void CountTest()
        {
            Assert.AreEqual(3, query.Count);
        }

        [Test]
        public void KeysTest()
        {
            ICollection<string> keys = new string[]
            {
                "key1",
                "key2",
                "key3"
            };
            CollectionAssert.AreEqual(keys, query.Keys);
        }

        [Test]
        public void ValuesTest()
        {
            ICollection<string> values = new string[]
            {
                "value1",
                "value2",
                "value3"
            };
            CollectionAssert.AreEqual(values, query.Values);
        }

        [Test]
        public void PairsTest()
        {
            ICollection<KeyValuePair<string, string>> pairs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2"),
                new KeyValuePair<string, string>("key3", "value3")
            };
            CollectionAssert.AreEqual(pairs, query.Pairs);
        }

        [Test]
        public void ClearTest()
        {
            query.Clear();
            Assert.AreEqual(0, query.Count);
        }

        [Test]
        public void ContainsKeyTest()
        {
            Assert.IsTrue(query.ContainsKey("key1"));
            Assert.IsFalse(query.ContainsKey("key4"));
        }

        [Test]
        public void GetTest()
        {
            Assert.AreEqual("value1", query.Get("key1"));
            Assert.IsNull(query.Get("key4"));
        }

        [Test]
        public void RemoveTest()
        {
            query.Remove("key1");
            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void SetTest()
        {
            query.Set("key4", "value4-1");
            Assert.AreEqual(4, query.Count);
            Assert.AreEqual("value4-1", query.Get("key4"));
            query.Set("key4", "value4-2");
            Assert.AreEqual(4, query.Count);
            Assert.AreEqual("value4-2", query.Get("key4"));
            query.Set("key5", 5);
            Assert.AreEqual("5", query.Get("key5"));
            Assert.Throws<ArgumentNullException>(() =>
            {
                query.Set(null, "value6");
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                query.Set("key6", null);
            });
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("key1=value1&key2=value2&key3=value3", query.ToString());
            query.Clear();
            query.Set("empty", "");
            query.Set("message", "'Hello, world!'");
            query.Set("math", "1 + 2 + 3 = 6");
            query.Set("logic", "A & B & C = D");
            query.Set("number", 42);
            Assert.AreEqual("empty=&message=%27Hello%2c+world!%27&math=1+%2b+2+%2b+3+%3d+6&logic=A+%26+B+%26+C+%3d+D&number=42", query.ToString());
        }
    }
}
