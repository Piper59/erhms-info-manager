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
    }
}
