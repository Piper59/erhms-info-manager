using ERHMS.Utility;
using NUnit.Framework;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class LogTest
    {
        private int GetLineCount(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream))
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
            int lineCount = GetLineCount(Log.FilePath);
            Log.Logger.Fatal(null);
            Assert.AreEqual(lineCount + 1, GetLineCount(Log.FilePath));
            Log.Logger.Fatal(null);
            Assert.AreEqual(lineCount + 2, GetLineCount(Log.FilePath));
            Log.Logger.Fatal(null);
            Assert.AreEqual(lineCount + 3, GetLineCount(Log.FilePath));
        }

        [Test]
        public void GetAndSetLevelNameTest()
        {
            string levelName = Log.GetLevelName();
            Log.SetLevelName("WARN");
            try
            {
                Assert.AreEqual("WARN", Log.GetLevelName());
                int lineCount = GetLineCount(Log.FilePath);
                Log.Logger.Debug(null);
                Assert.AreEqual(lineCount, GetLineCount(Log.FilePath));
                Log.Logger.Info(null);
                Assert.AreEqual(lineCount, GetLineCount(Log.FilePath));
                Log.Logger.Warn(null);
                Assert.AreEqual(lineCount + 1, GetLineCount(Log.FilePath));
                Log.Logger.Error(null);
                Assert.AreEqual(lineCount + 2, GetLineCount(Log.FilePath));
                Log.Logger.Fatal(null);
                Assert.AreEqual(lineCount + 3, GetLineCount(Log.FilePath));
            }
            finally
            {
                Log.SetLevelName(levelName);
            }
        }
    }
}
