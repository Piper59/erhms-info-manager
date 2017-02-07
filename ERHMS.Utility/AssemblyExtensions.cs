using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class AssemblyExtensions
    {
        public static DirectoryInfo GetEntryDirectory()
        {
            return new FileInfo(Assembly.GetEntryAssembly().Location).Directory;
        }

        public static Version GetVersion(this Assembly @this)
        {
            return @this.GetName().Version;
        }

        public static string GetManifestResourceText(this Assembly @this, string resourceName)
        {
            using (Stream stream = @this.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void CopyManifestResourceTo(this Assembly @this, string resourceName, FileInfo file)
        {
            using (Stream source = @this.GetManifestResourceStream(resourceName))
            using (Stream target = file.Create())
            {
                source.CopyTo(target);
            }
        }
    }
}
