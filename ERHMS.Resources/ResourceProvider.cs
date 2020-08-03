using System.IO;
using System.Reflection;

namespace ERHMS.Resources
{
    public static class ResourceProvider
    {
        public static Stream GetResource(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}
