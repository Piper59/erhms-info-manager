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
