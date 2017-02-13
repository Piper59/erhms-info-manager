using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class AssemblyExtensions
    {
        public static string GetEntryDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static string GetManifestResourceText(this Assembly @this, string resourceName)
        {
            using (Stream stream = @this.GetManifestResourceStream(resourceName))
            using (TextReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void CopyManifestResourceTo(this Assembly @this, string resourceName, string path)
        {
            using (Stream source = @this.GetManifestResourceStream(resourceName))
            using (Stream target = File.Create(path))
            {
                source.CopyTo(target);
            }
        }
    }
}
