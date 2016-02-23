using Epi;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static Configuration Current { get; private set; }

        public static void LoadConfiguration(string path)
        {
            Configuration.Load(path);
            Current = Configuration.GetNewInstance();
        }

        public static void LoadDefaultConfiguration()
        {
            if (!File.Exists(Configuration.DefaultConfigurationPath))
            {
                Configuration.Save(Configuration.CreateDefaultConfiguration());
            }
            LoadConfiguration(Configuration.DefaultConfigurationPath);
        }

        public static void Refresh()
        {
            LoadConfiguration(Current.ConfigFilePath);
        }
    }
}
