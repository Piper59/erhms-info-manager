using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class IOExtensionsTest
    {
        [Test]
        public void NormalizeExtensionTest()
        {
            Assert.AreEqual(".txt", IOExtensions.NormalizeExtension(".txt"));
            Assert.AreEqual(".txt", IOExtensions.NormalizeExtension("txt"));
            Assert.AreEqual("", IOExtensions.NormalizeExtension(""));
            Assert.AreEqual("", IOExtensions.NormalizeExtension(null));
        }

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
                FileInfo file1 = directory.TouchFile("file1.txt");
                FileInfo file2 = directory.TouchFile("file2.csv");
                FileInfo file3 = subdirectory.TouchFile("file3.txt");
                FileInfo file4 = subdirectory.TouchFile("file4.csv");
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
                subdirectory1.TouchFile("file1.txt");
                subsubdirectory1.TouchFile("file2.txt");
                DirectoryInfo subdirectory2 = directory.GetSubdirectory("subdirectory2");
                DirectoryInfo subsubdirectory2 = subdirectory2.GetSubdirectory("subsubdirectory");
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
            FileInfo file = IOExtensions.GetTemporaryFile(".txt");
            try
            {
                FileAssert.Exists(file);
                Assert.IsTrue(file.CreationTime.IsRecent());
                Assert.AreEqual(0, file.Length);
                Assert.AreEqual(".txt", file.Extension);
            }
            finally
            {
                file.Delete();
            }
        }
    }
}
