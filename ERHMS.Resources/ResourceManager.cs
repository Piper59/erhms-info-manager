using ERHMS.Domain;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;
using System.Reflection;

namespace ERHMS.Resources
{
    public static class ResourceManager
    {
        private static readonly Assembly assembly = typeof(ResourceManager).Assembly;

        public static string GetResourceName(CoreView coreView)
        {
            return $"Templates.Forms.{coreView.CoreProject}.{coreView.Name}.xml";
        }

        public static Stream GetStream(string resourceName)
        {
            return assembly.GetManifestResourceStream($"ERHMS.Resources.{resourceName}");
        }

        public static XTemplate GetXTemplate(string resourceName)
        {
            using (Stream stream = GetStream(resourceName))
            {
                return XTemplate.Load(stream);
            }
        }

        public static XTemplate GetXTemplate(CoreView coreView)
        {
            return GetXTemplate(GetResourceName(coreView));
        }
    }
}
