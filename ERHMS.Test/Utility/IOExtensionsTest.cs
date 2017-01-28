using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class IOExtensionsTest
    {
        private class FileInfoComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                {
                    if (x == null && y == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return x == null ? -1 : 1;
                    }
                }
                else
                {
                    return string.Compare(((FileInfo)x).FullName, ((FileInfo)y).FullName);
                }
            }
        }

        private static DirectoryInfo GetTemporaryDirectory(string methodName)
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), typeof(IOExtensionsTest).FullName, methodName));
        }

        [Test]
        public void NormalizeExtensionTest()
        {
            Assert.AreEqual(IOExtensions.NormalizeExtension(".txt"), ".txt");
            Assert.AreEqual(IOExtensions.NormalizeExtension("txt"), ".txt");
            Assert.AreEqual(IOExtensions.NormalizeExtension(""), "");
            Assert.AreEqual(IOExtensions.NormalizeExtension(null), "");
        }

        [Test]
        public void SearchByExtensionTest()
        {
            DirectoryInfo directory = GetTemporaryDirectory("SearchByExtensionTest");
            try
            {
                DirectoryInfo subdirectory = directory.CreateSubdirectory("subdirectory");
                FileInfo file1 = new FileInfo(Path.Combine(directory.FullName, "file1.txt"));
                FileInfo file2 = new FileInfo(Path.Combine(directory.FullName, "file2.csv"));
                FileInfo file3 = new FileInfo(Path.Combine(subdirectory.FullName, "file3.txt"));
                FileInfo file4 = new FileInfo(Path.Combine(subdirectory.FullName, "file4.csv"));
                using (file1.Create()) { }
                using (file2.Create()) { }
                using (file3.Create()) { }
                using (file4.Create()) { }
                FileInfo[] textFiles = new FileInfo[]
                {
                    file1,
                    file3
                };
                CollectionAssert.AreEqual(directory.SearchByExtension(".txt"), textFiles, new FileInfoComparer());
            }
            finally
            {
                directory.Refresh();
                if (directory.Exists)
                {
                    directory.Delete(true);
                }
            }
        }

        [Test]
        public void CopyToTest()
        {
            DirectoryInfo directory = GetTemporaryDirectory("CopyToTest");
            try
            {
                DirectoryInfo subdirectory1 = directory.CreateSubdirectory("subdirectory1");
                DirectoryInfo subsubdirectory1 = subdirectory1.CreateSubdirectory("subsubdirectory");
                using (File.Create(Path.Combine(subdirectory1.FullName, "file1.txt"))) { }
                using (File.Create(Path.Combine(subsubdirectory1.FullName, "file2.txt"))) { }
                DirectoryInfo subdirectory2 = new DirectoryInfo(Path.Combine(directory.FullName, "subdirectory2"));
                DirectoryInfo subsubdirectory2 = new DirectoryInfo(Path.Combine(subdirectory2.FullName, "subsubdirectory"));
                subdirectory1.CopyTo(subdirectory2, true);
                FileAssert.Exists(Path.Combine(subdirectory2.FullName, "file1.txt"));
                FileAssert.Exists(Path.Combine(subsubdirectory2.FullName, "file2.txt"));
            }
            finally
            {
                directory.Refresh();
                if (directory.Exists)
                {
                    directory.Delete(true);
                }
            }
        }

        [Test]
        public void GetTemporaryFileTest()
        {
            FileInfo file = IOExtensions.GetTemporaryFile(".txt");
            FileAssert.Exists(file);
            Assert.LessOrEqual((DateTime.Now - file.CreationTime).TotalSeconds, 1.0);
            Assert.AreEqual(file.Length, 0);
            Assert.AreEqual(file.Extension, ".txt");
        }
    }
}
