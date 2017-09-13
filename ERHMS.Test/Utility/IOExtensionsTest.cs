using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.Test.Utility
{
    public class IOExtensionsTest
    {
        private static readonly ICollection<string[]> PathArrays = new string[][]
        {
            new string[] { "file1.txt" },
            new string[] { "file2.csv" },
            new string[] { "subdirectory", "file3.txt" },
            new string[] { "subdirectory", "file4.csv" }
        };

        [Test]
        public void GetTempFileNameTest()
        {
            GetTempFileNameTest("temp_{0:N}.txt");
            GetTempFileNameTest("{1}{0:N}{2}", "temp_", ".txt");
        }

        private void GetTempFileNameTest(string format, params object[] args)
        {
            string path = null;
            try
            {
                path = IOExtensions.GetTempFileName(format, args);
                FileInfo file = new FileInfo(path);
                FileAssert.Exists(file);
                Assert.LessOrEqual(DateTime.Now - file.CreationTime, TimeSpan.FromSeconds(1.0));
                Assert.AreEqual(0, file.Length);
                StringAssert.IsMatch(@"^temp_[0-9a-f]{32}\.txt$", file.Name);
            }
            finally
            {
                if (path != null)
                {
                    File.Delete(path);
                }
            }
        }

        private void CreateFiles(TempDirectory directory)
        {
            foreach (string[] paths in PathArrays)
            {
                directory.CreateFile(paths);
            }
        }

        [Test]
        public void CopyDirectoryTest()
        {
            using (TempDirectory original = new TempDirectory(nameof(CopyDirectoryTest)))
            using (TempDirectory copy = new TempDirectory(nameof(CopyDirectoryTest) + "Copy"))
            {
                CreateFiles(original);
                IOExtensions.CopyDirectory(original.FullName, copy.FullName);
                foreach (string[] paths in PathArrays)
                {
                    FileAssert.Exists(copy.CombinePaths(paths));
                }
            }
        }

        [Test]
        public void SearchByExtensionTest()
        {
            using (TempDirectory directory = new TempDirectory(nameof(SearchByExtensionTest)))
            {
                CreateFiles(directory);
                ICollection<string> expected = new string[]
                {
                    directory.CombinePaths("file1.txt"),
                    directory.CombinePaths("subdirectory", "file3.txt")
                };
                IEnumerable<string> actual = new DirectoryInfo(directory.FullName).SearchByExtension(".txt").Select(file => file.FullName);
                CollectionAssert.AreEquivalent(expected, actual);
            }
        }
    }
}
