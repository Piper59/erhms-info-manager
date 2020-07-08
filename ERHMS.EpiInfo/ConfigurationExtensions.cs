using Epi;
using Epi.DataSets;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static bool Exists()
        {
            return File.Exists(Configuration.DefaultConfigurationPath);
        }

        public static Configuration Create()
        {
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            return configuration;
        }

        public static void SetFipsCrypto(this Configuration @this)
        {
            Config.TextEncryptionModuleDataTable table = @this.ConfigDataSet.TextEncryptionModule;
            table.Clear();
            Config.TextEncryptionModuleRow row = table.NewTextEncryptionModuleRow();
            row.FileName = "FipsCrypto.dll";
            table.Rows.Add(row);
        }

        public static void Save(this Configuration @this)
        {
            Configuration.Save(@this);
        }

        public static Configuration Load()
        {
            Configuration.Load(Configuration.DefaultConfigurationPath);
            return Configuration.GetNewInstance();
        }
    }
}
