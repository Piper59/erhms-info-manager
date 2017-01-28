using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using SearchOption = System.IO.SearchOption;

namespace ERHMS.Utility
{
    public static class IOExtensions
    {
        public static string NormalizeExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return "";
            }
            else
            {
                return extension.StartsWith(".") ? extension : string.Format(".{0}", extension);
            }
        }

        public static FileInfo GetFile(this DirectoryInfo @this, params string[] paths)
        {
            return new FileInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static DirectoryInfo GetSubdirectory(this DirectoryInfo @this, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static IEnumerable<FileInfo> SearchByExtension(this DirectoryInfo @this, string extension)
        {
            return @this.EnumerateFiles(string.Format("*{0}", NormalizeExtension(extension)), SearchOption.AllDirectories);
        }

        public static void CopyTo(this DirectoryInfo @this, DirectoryInfo target, bool overwrite)
        {
            target.Create();
            foreach (FileInfo sourceFile in @this.GetFiles())
            {
                FileInfo targetFile = target.GetFile(sourceFile.Name);
                if (!targetFile.Exists || overwrite)
                {
                    sourceFile.CopyTo(targetFile.FullName, overwrite);
                }
            }
            foreach (DirectoryInfo subsource in @this.GetDirectories())
            {
                DirectoryInfo subtarget = target.GetSubdirectory(subsource.Name);
                subsource.CopyTo(subtarget, overwrite);
            }
        }

        public static void Recycle(this FileInfo @this)
        {
            FileSystem.DeleteFile(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public static void Recycle(this DirectoryInfo @this)
        {
            FileSystem.DeleteDirectory(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public static FileInfo GetTemporaryFile(string extension)
        {
            extension = NormalizeExtension(extension);
            string path = Path.GetTempPath();
            FileInfo file;
            do
            {
                string fileName = string.Format("ERHMS_{0:N}{1}", Guid.NewGuid(), extension);
                file = new FileInfo(Path.Combine(path, fileName));
            } while (file.Exists);
            using (file.OpenWrite()) { }
            file.Refresh();
            return file;
        }
    }
}
