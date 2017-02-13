using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using SearchOption = System.IO.SearchOption;

namespace ERHMS.Utility
{
    public static class IOExtensions
    {
        public static string GetTempFileName(string format, params object[] args)
        {
            object[] argsWithGuid = new object[args.Length + 1];
            args.CopyTo(argsWithGuid, 1);
            string directoryPath = Path.GetTempPath();
            string filePath;
            do
            {
                argsWithGuid[0] = Guid.NewGuid();
                filePath = Path.Combine(directoryPath, string.Format(format, argsWithGuid));
            } while (File.Exists(filePath));
            using (File.Create(filePath)) { }
            return filePath;
        }

        public static void RecycleFile(string path)
        {
            FileSystem.DeleteFile(path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            FileSystem.CopyDirectory(sourcePath, targetPath, UIOption.AllDialogs);
        }

        public static IEnumerable<FileInfo> Search(this DirectoryInfo @this, string extension)
        {
            return @this.EnumerateFiles("*" + extension, SearchOption.AllDirectories);
        }
    }
}
