using ERHMS.Domain;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;

namespace ERHMS.Resources
{
    public static class ResourceManager
    {
        public static Stream GetStream(string resourceName)
        {
            return typeof(ResourceManager).Assembly.GetManifestResourceStream(typeof(ResourceManager), resourceName);
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
            return GetXTemplate($"Templates.Forms.{coreView.CoreProject}.{coreView.Name}.xml");
        }
    }
}
