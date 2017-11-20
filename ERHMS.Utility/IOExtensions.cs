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
            ArrayExtensions.Resize(ref args, args.Length + 1, 1);
            string directoryPath = Path.GetTempPath();
            string filePath;
            do
            {
                args[0] = Guid.NewGuid();
                filePath = Path.Combine(directoryPath, string.Format(format, args));
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
            FileSystem.CopyDirectory(sourcePath, targetPath, UIOption.AllDialogs, UICancelOption.DoNothing);
        }

        public static IEnumerable<FileInfo> SearchByExtension(this DirectoryInfo @this, string extension)
        {
            return @this.EnumerateFiles("*" + extension, SearchOption.AllDirectories);
        }
    }
}
