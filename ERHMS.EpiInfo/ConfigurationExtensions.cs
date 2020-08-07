using Epi;
using Epi.DataSets;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        private const string FipsCryptoAssembly = "FipsCrypto.dll";

        public static string FilePath => Configuration.DefaultConfigurationPath;

        public static bool Exists()
        {
            return File.Exists(FilePath);
        }

        public static Configuration Create(string path = null)
        {
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            if (path != null)
            {
                configuration = new Configuration(path, configuration.ConfigDataSet);
            }
            return configuration;
        }

        public static void SetFipsCrypto(this Configuration @this)
        {
            Config.TextEncryptionModuleDataTable table = @this.ConfigDataSet.TextEncryptionModule;
            table.Clear();
            Config.TextEncryptionModuleRow row = table.NewTextEncryptionModuleRow();
            row.FileName = FipsCryptoAssembly;
            table.Rows.Add(row);
        }

        public static void Save(this Configuration @this) => Configuration.Save(@this);

        public static Configuration Load(string path = null)
        {
            Configuration.Load(path ?? FilePath);
            return Configuration.GetNewInstance();
        }
    }
}
