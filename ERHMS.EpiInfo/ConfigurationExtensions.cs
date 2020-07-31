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

        public static Configuration Create(string filePath = null)
        {
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            if (filePath != null)
            {
                configuration = new Configuration(filePath, configuration.ConfigDataSet);
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

        public static void Save(this Configuration @this)
        {
            Configuration.Save(@this);
        }

        public static Configuration Load(string filePath = null)
        {
            Configuration.Load(filePath ?? FilePath);
            return Configuration.GetNewInstance();
        }
    }
}
