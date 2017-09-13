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

        [Test]
        public void LoggerTest()
        {
            FileAssert.Exists(Log.FilePath);
            int count = GetLineCount();
            Log.Logger.Debug(null);
            count++;
            Assert.AreEqual(count, GetLineCount());
            Log.Logger.Info(null);
            count++;
            Assert.AreEqual(count, GetLineCount());
            Log.Logger.Warn(null);
            count++;
            Assert.AreEqual(count, GetLineCount());
            Log.Logger.Error(null);
            count++;
            Assert.AreEqual(count, GetLineCount());
            Log.Logger.Fatal(null);
            count++;
            Assert.AreEqual(count, GetLineCount());
        }

        [Test]
        public void LevelNameTest()
        {
            string levelName = Log.LevelName;
            Log.LevelName = "WARN";
            try
            {
                int count = GetLineCount();
                Log.Logger.Debug(null);
                Assert.AreEqual(count, GetLineCount());
                Log.Logger.Info(null);
                Assert.AreEqual(count, GetLineCount());
                Log.Logger.Warn(null);
                count++;
                Assert.AreEqual(count, GetLineCount());
                Log.Logger.Error(null);
                count++;
                Assert.AreEqual(count, GetLineCount());
                Log.Logger.Fatal(null);
                count++;
                Assert.AreEqual(count, GetLineCount());
            }
            finally
            {
                Log.LevelName = levelName;
            }
        }
    }
}
