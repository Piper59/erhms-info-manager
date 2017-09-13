using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class DamerauLevenshteinTest
    {
        [Test]
        public void GetEditDistanceTest()
        {
            GetEditDistanceTest(0, null, null);
            GetEditDistanceTest(0, null, "");
            GetEditDistanceTest(0, "", "");
            GetEditDistanceTest(0, "test", "test");
            GetEditDistanceTest(0, "test", "TEST");
            GetEditDistanceTest(1, "test", "tests");
            GetEditDistanceTest(2, "test", "mytest");
            GetEditDistanceTest(3, "test", "mytests");
            GetEditDistanceTest(1, "test", "teest");
            GetEditDistanceTest(1, "test", "tst");
            GetEditDistanceTest(1, "test", "tset");
            GetEditDistanceTest(2, "test", "tsets");
            GetEditDistanceTest(1, "banana", "abnana");
            GetEditDistanceTest(1, "banana", "baanna");
            GetEditDistanceTest(2, "banana", "baanan");
        }

        private void GetEditDistanceTest(int expected, string str1, string str2)
        {
            Assert.AreEqual(expected, DamerauLevenshtein.GetEditDistance(str1, str2));
        }
    }
}
