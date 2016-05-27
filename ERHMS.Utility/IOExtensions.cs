using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using SearchOption = System.IO.SearchOption;

namespace ERHMS.Utility
{
    public static class IOExtensions
    {
        public static FileInfo GetFile(this DirectoryInfo @this, params string[] paths)
        {
            return new FileInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static DirectoryInfo GetSubdirectory(this DirectoryInfo @this, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static IEnumerable<FileInfo> SearchByExtension(this DirectoryInfo @this, string extension, bool recurse = true)
        {
            return @this.EnumerateFiles(
                string.Format("*{0}", extension),
                recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public static void Copy(DirectoryInfo source, DirectoryInfo target, bool overwrite = false)
        {
            target.Create();
            foreach (FileInfo sourceFile in source.GetFiles())
            {
                FileInfo targetFile = target.GetFile(sourceFile.Name);
                if (targetFile.Exists && !overwrite)
                {
                    continue;
                }
                sourceFile.CopyTo(targetFile.FullName, overwrite);
            }
            foreach (DirectoryInfo subsource in source.GetDirectories())
            {
                DirectoryInfo subtarget = target.GetSubdirectory(subsource.Name);
                Copy(subsource, subtarget);
            }
        }

        public static FileInfo GetTemporaryFile(string prefix = "ERHMS_", string extension = null)
        {
            if (extension != null && !extension.StartsWith("."))
            {
                extension = string.Format(".{0}", extension);
            }
            string path = Path.GetTempPath();
            FileInfo file;
            do
            {
                string fileName = string.Format("{0}{1:N}{2}", prefix, Guid.NewGuid(), extension);
                file = new FileInfo(Path.Combine(path, fileName));
            } while (file.Exists);
            using (file.OpenWrite()) { }
            return file;
        }

        public static void Recycle(this FileInfo @this)
        {
            FileSystem.DeleteFile(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public static void Recycle(this DirectoryInfo @this)
        {
            FileSystem.DeleteDirectory(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
    }
}
