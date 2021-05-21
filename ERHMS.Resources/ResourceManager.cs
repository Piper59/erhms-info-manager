using ERHMS.Common.Reflection;
using ERHMS.Domain;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;

namespace ERHMS.Resources
{
    public static class ResourceManager
    {
        public static string GetResourceName(CoreView coreView)
        {
            return $"Templates.Forms.{coreView.CoreProject}.{coreView.Name}.xml";
        }

        public static Stream GetStream(string resourceName)
        {
            return typeof(ResourceManager).GetManifestResourceStream(resourceName);
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
