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
                int lineCount = 0;
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
                return lineCount;
            }
        }

        [Test]
        public void LoggerTest()
        {
            FileAssert.Exists(Log.FilePath);
            int lineCount = GetLineCount();
            Log.Logger.Debug(null);
            Assert.AreEqual(lineCount + 1, GetLineCount());
            Log.Logger.Debug(null);
            Assert.AreEqual(lineCount + 2, GetLineCount());
            Log.Logger.Debug(null);
            Assert.AreEqual(lineCount + 3, GetLineCount());
        }

        [Test]
        public void LevelNameTest()
        {
            string levelName = Log.LevelName;
            Log.LevelName = "WARN";
            try
            {
                int lineCount = GetLineCount();
                Log.Logger.Debug(null);
                Assert.AreEqual(lineCount, GetLineCount());
                Log.Logger.Info(null);
                Assert.AreEqual(lineCount, GetLineCount());
                Log.Logger.Warn(null);
                Assert.AreEqual(lineCount + 1, GetLineCount());
                Log.Logger.Error(null);
                Assert.AreEqual(lineCount + 2, GetLineCount());
                Log.Logger.Fatal(null);
                Assert.AreEqual(lineCount + 3, GetLineCount());
            }
            finally
            {
                Log.LevelName = levelName;
            }
        }
    }
}
