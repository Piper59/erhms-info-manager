using ERHMS.Utility;
using NUnit.Framework;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class QueryStringTest
    {
        [Test]
        public void ContainsKey()
        {
            QueryString queryString = new QueryString("key=value");
            Assert.IsTrue(queryString.ContainsKey("key"));
            Assert.IsFalse(queryString.ContainsKey("value"));
        }

        [Test]
        public void GetTest()
        {
            QueryString queryString = new QueryString("key1=value1&key2=value2&key3=value3");
            Assert.AreEqual("value1", queryString.Get("key1"));
            Assert.AreEqual("value2", queryString.Get("key2"));
            Assert.AreEqual("value3", queryString.Get("key3"));
            Assert.IsNull(queryString.Get("key4"));
        }

        [Test]
        public void SetTest()
        {
            QueryString queryString = new QueryString();
            Assert.Catch(() =>
            {
                queryString.Set(null, "value");
            });
            Assert.Catch(() =>
            {
                queryString.Set("key", null);
            });
            Assert.AreEqual(0, queryString.Count());
            queryString.Set("key", "value");
            Assert.AreEqual("value", queryString.Get("key"));
            queryString.Set("key", 1);
            Assert.AreEqual("1", queryString.Get("key"));
            queryString.Set("key", true);
            Assert.AreEqual("True", queryString.Get("key"));
            Assert.AreEqual(1, queryString.Count());
        }

        [Test]
        public void ToStringTest()
        {
            QueryString queryString = new QueryString();
            queryString.Set("empty", "");
            queryString.Set("message", "'Hello, world!'");
            queryString.Set("math", "1 + 2 + 3 = 6");
            queryString.Set("logic", "A & B & C = D");
            queryString.Set("number", 42);
            string value = "empty=&message=%27Hello%2c+world!%27&math=1+%2b+2+%2b+3+%3d+6&logic=A+%26+B+%26+C+%3d+D&number=42";
            Assert.AreEqual(value, queryString.ToString());
        }
    }
}
