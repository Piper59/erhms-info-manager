using Epi.DataSets;
using System.Configuration;
using System.IO;
using Configuration = Epi.Configuration;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        private const string FipsCryptoFileName = "FipsCrypto.dll";

        public static string FilePath => Configuration.DefaultConfigurationPath;

        public static bool CompatibilityMode
        {
            get
            {
                string value = ConfigurationManager.AppSettings[nameof(CompatibilityMode)];
                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }
                return false;
            }
        }

        public static bool Exists()
        {
            return File.Exists(FilePath);
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
            row.FileName = FipsCryptoFileName;
            table.Rows.Add(row);
        }

        public static void Save(this Configuration @this)
        {
            Configuration.Save(@this);
        }

        public static Configuration Load()
        {
            Configuration.Load(FilePath);
            return Configuration.GetNewInstance();
        }
    }
}
