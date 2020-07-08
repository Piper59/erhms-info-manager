using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        public static string GetEntryDir()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }
}
