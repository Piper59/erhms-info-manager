using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System.IO;

namespace ERHMS.Test.EpiInfo
{
    public class LogTest
    {
        private int GetLineCount(FileInfo file)
        {
            using (Stream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
        public void CurrentTest()
        {
            FileInfo file = Log.GetFile();
            FileAssert.Exists(file);
            int lineCount = GetLineCount(file);
            Log.Current.Fatal(null);
            Assert.AreEqual(lineCount + 1, GetLineCount(file));
            Log.Current.Fatal(null);
            Assert.AreEqual(lineCount + 2, GetLineCount(file));
            Log.Current.Fatal(null);
            Assert.AreEqual(lineCount + 3, GetLineCount(file));
        }

        [Test]
        public void SuspendAndResumeTest()
        {
            FileInfo file = Log.GetFile();
            FileAssert.Exists(file);
            Log.Suspend();
            file.Delete();
            file.Refresh();
            FileAssert.DoesNotExist(file);
            Log.Current.Fatal(null);
            file.Refresh();
            FileAssert.DoesNotExist(file);
            Log.Resume();
            CurrentTest();
        }

        [Test]
        public void SetDirectoryTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => SetDirectoryTest());
            try
            {
                Assert.AreNotEqual(directory.FullName, Log.GetFile().DirectoryName);
                Log.SetDirectory(directory);
                Assert.AreEqual(directory.FullName, Log.GetFile().DirectoryName);
                CurrentTest();
            }
            finally
            {
                Log.SetDirectory(Log.GetDefaultDirectory());
                directory.Delete(true);
            }
        }

        [Test]
        public void SetLevelNameTest()
        {
            Log.SetLevelName("WARN");
            try
            {
                FileInfo file = Log.GetFile();
                int lineCount = GetLineCount(file);
                Log.Current.Debug(null);
                Assert.AreEqual(lineCount, GetLineCount(file));
                Log.Current.Info(null);
                Assert.AreEqual(lineCount, GetLineCount(file));
                Log.Current.Warn(null);
                Assert.AreEqual(lineCount + 1, GetLineCount(file));
                Log.Current.Error(null);
                Assert.AreEqual(lineCount + 2, GetLineCount(file));
                Log.Current.Fatal(null);
                Assert.AreEqual(lineCount + 3, GetLineCount(file));
            }
            finally
            {
                Log.SetLevelName(Settings.Default.LogLevel);
            }
        }
    }
}
