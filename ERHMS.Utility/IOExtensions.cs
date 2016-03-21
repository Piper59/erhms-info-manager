using Microsoft.VisualBasic.FileIO;
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
