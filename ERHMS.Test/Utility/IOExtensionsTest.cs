using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class IOExtensionsTest
    {
        [Test]
        public void GetTempFileNameTest()
        {
            string path1 = null;
            string path2 = null;
            try
            {
                path1 = IOExtensions.GetTempFileName("temp_{0:N}.txt");
                path2 = IOExtensions.GetTempFileName("{1}{0:N}{2}", "temp_", ".txt");
                GetTempFileNameTest(path1);
                GetTempFileNameTest(path2);
            }
            finally
            {
                if (path1 != null)
                {
                    File.Delete(path1);
                }
                if (path2 != null)
                {
                    File.Delete(path2);
                }
            }
        }

        private void GetTempFileNameTest(string path)
        {
            FileInfo file = new FileInfo(path);
            FileAssert.Exists(file);
            Assert.IsTrue(file.CreationTime.IsRecent());
            Assert.AreEqual(0, file.Length);
            StringAssert.IsMatch(@"^temp_[0-9a-f]{32}\.txt$", file.Name);
        }

        private void CreateFiles(TempDirectory directory)
        {
            directory.CreateFile("file1.txt");
            directory.CreateFile("file2.csv");
            directory.CreateFile("subdirectory", "file3.txt");
            directory.CreateFile("subdirectory", "file4.csv");
        }

        [Test]
        public void CopyDirectoryTest()
        {
            using (TempDirectory directory1 = new TempDirectory(nameof(CopyDirectoryTest) + "1"))
            using (TempDirectory directory2 = new TempDirectory(nameof(CopyDirectoryTest) + "2"))
            {
                CreateFiles(directory1);
                IOExtensions.CopyDirectory(directory1.Path, directory2.Path);
                FileAssert.Exists(directory2.CombinePaths("file1.txt"));
                FileAssert.Exists(directory2.CombinePaths("file2.csv"));
                FileAssert.Exists(directory2.CombinePaths("subdirectory", "file3.txt"));
                FileAssert.Exists(directory2.CombinePaths("subdirectory", "file4.csv"));
            }
        }

        [Test]
        public void SearchTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(SearchTest)))
            {
                CreateFiles(directory);
                ICollection<string> paths = new string[]
                {
                    directory.CombinePaths("file1.txt"),
                    directory.CombinePaths("subdirectory", "file3.txt")
                };
                CollectionAssert.AreEqual(paths, new DirectoryInfo(directory.Path).Search(".txt").Select(file => file.FullName));
            }
        }
    }
}
