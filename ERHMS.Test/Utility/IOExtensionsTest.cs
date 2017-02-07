using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ERHMS.Test.Utility
{
    public class IOExtensionsTest
    {
        [Test]
        public void TouchTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => TouchTest());
            try
            {
                string fileName = "file.txt";
                FileInfo file = directory.GetFile(fileName);
                file.Touch();
                FileAssert.Exists(file);
                Assert.IsTrue(file.CreationTime.IsRecent());
                Assert.AreEqual(0, file.Length);
                string content = "Hello, world!";
                File.WriteAllText(file.FullName, content);
                Assert.AreEqual(content, File.ReadAllText(file.FullName));
                Assert.DoesNotThrow(() =>
                {
                    file.Touch();
                });
                Assert.AreEqual(content, File.ReadAllText(file.FullName));
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Test]
        public void SearchByExtensionTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => SearchByExtensionTest());
            try
            {
                DirectoryInfo subdirectory = directory.CreateSubdirectory("subdirectory");
                FileInfo file1 = directory.GetFile("file1.txt");
                FileInfo file2 = directory.GetFile("file2.csv");
                FileInfo file3 = subdirectory.GetFile("file3.txt");
                FileInfo file4 = subdirectory.GetFile("file4.csv");
                file1.Touch();
                file2.Touch();
                file3.Touch();
                file4.Touch();
                ICollection<FileInfo> textFiles = new FileInfo[]
                {
                    file1,
                    file3
                };
                CollectionAssert.AreEqual(textFiles, directory.SearchByExtension(".txt"), new FileInfoComparer());
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Test]
        public void CopyToTest()
        {
            DirectoryInfo directory = Helpers.GetTemporaryDirectory(() => CopyToTest());
            try
            {
                DirectoryInfo subdirectory1 = directory.CreateSubdirectory("subdirectory1");
                DirectoryInfo subsubdirectory1 = subdirectory1.CreateSubdirectory("subsubdirectory");
                subdirectory1.GetFile("file1.txt").Touch();
                subsubdirectory1.GetFile("file2.txt").Touch();
                DirectoryInfo subdirectory2 = directory.GetDirectory("subdirectory2");
                DirectoryInfo subsubdirectory2 = subdirectory2.GetDirectory("subsubdirectory");
                subdirectory1.CopyTo(subdirectory2, true);
                FileAssert.Exists(subdirectory2.GetFile("file1.txt"));
                FileAssert.Exists(subsubdirectory2.GetFile("file2.txt"));
            }
            finally
            {
                directory.Delete(true);
            }
        }

        [Test]
        public void GetTemporaryFileTest()
        {
            FileInfo file = IOExtensions.GetTemporaryFile("ERHMS_{0:N}.txt");
            try
            {
                FileAssert.Exists(file);
                Assert.IsTrue(file.CreationTime.IsRecent());
                Assert.AreEqual(0, file.Length);
                Assert.IsTrue(Regex.IsMatch(file.Name, @"^ERHMS_[0-9a-f]{32}\.txt$"));
            }
            finally
            {
                file.Delete();
            }
        }
    }
}
