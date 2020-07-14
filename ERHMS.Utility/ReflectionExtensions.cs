using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        public static string GetEntryDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static void CopyManifestResourceTo(this Assembly @this, string resourceName, Stream stream)
        {
            using (Stream source = @this.GetManifestResourceStream(resourceName))
            {
                source.CopyTo(stream);
            }
        }

        public static void CopyManifestResourceTo(this Assembly @this, string resourceName, string path)
        {
            using (Stream stream = File.Create(path))
            {
                @this.CopyManifestResourceTo(resourceName, stream);
            }
        }
    }
}
