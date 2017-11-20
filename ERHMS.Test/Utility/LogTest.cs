using ERHMS.Utility;
using NUnit.Framework;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class LogTest
    {
        public static int GetLineCount()
        {
            using (Stream stream = new FileStream(Log.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (TextReader reader = new StreamReader(stream))
            {
                int count = 0;
                while (reader.ReadLine() != null)
                {
                    count++;
                }
                return count;
            }
        }

        public static void LineCountTest(int expected)
        {
            Assert.AreEqual(expected, GetLineCount());
        }

        [Test]
        public void LoggerTest()
        {
            FileAssert.Exists(Log.FilePath);
            int count = GetLineCount();
            Log.Logger.Debug(null);
            LineCountTest(++count);
            Log.Logger.Info(null);
            LineCountTest(++count);
            Log.Logger.Warn(null);
            LineCountTest(++count);
            Log.Logger.Error(null);
            LineCountTest(++count);
            Log.Logger.Fatal(null);
            LineCountTest(++count);
        }

        [Test]
        public void LevelNameTest()
        {
            Log.LevelName = "WARN";
            try
            {
                int count = GetLineCount();
                Log.Logger.Debug(null);
                LineCountTest(count);
                Log.Logger.Info(null);
                LineCountTest(count);
                Log.Logger.Warn(null);
                LineCountTest(++count);
                Log.Logger.Error(null);
                LineCountTest(++count);
                Log.Logger.Fatal(null);
                LineCountTest(++count);
            }
            finally
            {
                Log.LevelName = "DEBUG";
            }
        }
    }
}
