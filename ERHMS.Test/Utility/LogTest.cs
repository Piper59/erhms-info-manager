using ERHMS.Utility;
using NUnit.Framework;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class LogTest
    {
        private int GetLineCount()
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
            Assert.AreEqual(count + 1, GetLineCount());
            Log.Logger.Debug(null);
            Assert.AreEqual(count + 2, GetLineCount());
            Log.Logger.Debug(null);
            Assert.AreEqual(count + 3, GetLineCount());
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
                Assert.AreEqual(count + 1, GetLineCount());
                Log.Logger.Error(null);
                Assert.AreEqual(count + 2, GetLineCount());
                Log.Logger.Fatal(null);
                Assert.AreEqual(count + 3, GetLineCount());
            }
            finally
            {
                Log.LevelName = levelName;
            }
        }
    }
}
