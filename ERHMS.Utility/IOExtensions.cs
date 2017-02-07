using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SearchOption = System.IO.SearchOption;

namespace ERHMS.Utility
{
    public static class IOExtensions
    {
        public static void Touch(this FileInfo @this)
        {
            using (@this.OpenWrite()) { }
            @this.Refresh();
        }

        public static FileInfo GetFile(this DirectoryInfo @this, params string[] paths)
        {
            return new FileInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo @this, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(@this.FullName, Path.Combine(paths)));
        }

        public static IEnumerable<FileInfo> SearchByExtension(this DirectoryInfo @this, string extension)
        {
            return @this.EnumerateFiles(string.Format("*{0}", extension), SearchOption.AllDirectories);
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
                DirectoryInfo subtarget = target.GetDirectory(subsource.Name);
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

        public static FileInfo GetTemporaryFile(string format, params object[] args)
        {
            string path = Path.GetTempPath();
            FileInfo file;
            do
            {
                string fileName = string.Format(format, args.Prepend(Guid.NewGuid()).ToArray());
                file = new FileInfo(Path.Combine(path, fileName));
            } while (file.Exists);
            file.Touch();
            return file;
        }
    }
}
