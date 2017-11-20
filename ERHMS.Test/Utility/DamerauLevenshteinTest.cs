using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class DamerauLevenshteinTest
    {
        [Test]
        public void GetDistanceTest()
        {
            GetDistanceTest(0, null, null);
            GetDistanceTest(0, null, "");
            GetDistanceTest(0, "", "");
            GetDistanceTest(0, "test", "test");
            GetDistanceTest(0, "test", "TEST");
            GetDistanceTest(1, "test", "tests");
            GetDistanceTest(2, "test", "mytest");
            GetDistanceTest(3, "test", "mytests");
            GetDistanceTest(1, "test", "teest");
            GetDistanceTest(1, "test", "tst");
            GetDistanceTest(1, "test", "tset");
            GetDistanceTest(2, "test", "tsets");
            GetDistanceTest(1, "banana", "abnana");
            GetDistanceTest(1, "banana", "baanna");
            GetDistanceTest(2, "banana", "baanan");
        }

        private void GetDistanceTest(int expected, string str1, string str2)
        {
            Assert.AreEqual(expected, DamerauLevenshtein.GetDistance(str1, str2));
        }
    }
}
