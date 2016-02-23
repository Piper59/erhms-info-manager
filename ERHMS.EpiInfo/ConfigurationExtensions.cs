using Epi;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static void LoadDefaultConfiguration()
        {
            if (!File.Exists(Configuration.DefaultConfigurationPath))
            {
                Configuration configuration = Configuration.CreateDefaultConfiguration();
                Configuration.Save(configuration);
            }
            Configuration.Load(Configuration.DefaultConfigurationPath);
        }
    }
}
