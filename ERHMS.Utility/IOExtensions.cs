using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;

namespace ERHMS.Utility
{
    public static class IOExtensions
    {
        public static FileInfo GetFile(this DirectoryInfo @this, string path)
        {
            return new FileInfo(Path.Combine(@this.FullName, path));
        }

        public static DirectoryInfo GetSubdirectory(this DirectoryInfo @this, string path)
        {
            return new DirectoryInfo(Path.Combine(@this.FullName, path));
        }

        public static FileInfo GetTemporaryFile(string prefix = "ERHMS_", string extension = null)
        {
            FileInfo file;
            do
            {
                string fileName = string.Format("{0}{1:N}", prefix, Guid.NewGuid());
                fileName = Path.ChangeExtension(fileName, extension);
                file = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));
            } while (file.Exists);
            using (file.OpenWrite())
            { }
            return file;
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

        public static void Recycle(this DirectoryInfo @this)
        {
            FileSystem.DeleteDirectory(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        public static void Recycle(this FileInfo @this)
        {
            FileSystem.DeleteFile(@this.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
    }
}
