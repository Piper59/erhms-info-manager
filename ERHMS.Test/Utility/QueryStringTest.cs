using ERHMS.Utility;
using NUnit.Framework;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class QueryStringTest
    {
        [Test]
        public void SetTest()
        {
            QueryString query = new QueryString();
            Assert.Catch(() =>
            {
                query.Set(null, "value");
            });
            Assert.Catch(() =>
            {
                query.Set("key", null);
            });
            Assert.AreEqual(0, query.Count());
            query.Set("key", "");
            query.Set("key", 0);
            query.Set("key", new object());
            Assert.AreEqual(1, query.Count());
        }

        [Test]
        public void ToStringTest()
        {
            QueryString query = new QueryString();
            query.Set("empty", "");
            query.Set("message", "'Hello, world!'");
            query.Set("math", "1 + 2 + 3 = 6");
            query.Set("logic", "A & B & C = D");
            query.Set("number", 42);
            Assert.AreEqual("empty=&message=%27Hello%2c+world!%27&math=1+%2b+2+%2b+3+%3d+6&logic=A+%26+B+%26+C+%3d+D&number=42", query.ToString());
        }
    }
}
