using Epi;
using Epi.DataSets;
using ERHMS.Common.Logging;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static bool Exists()
        {
            return File.Exists(Configuration.DefaultConfigurationPath);
        }

        public static Configuration Create(string path = null)
        {
            if (path == null)
            {
                path = Configuration.DefaultConfigurationPath;
            }
            Log.Instance.Debug($"Creating Epi Info configuration: {path}");
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            return new Configuration(path, configuration.ConfigDataSet);
        }

        public static void SetTextEncryptionModule(this Configuration @this, bool fipsCompliant)
        {
            Config.TextEncryptionModuleDataTable table = @this.ConfigDataSet.TextEncryptionModule;
            table.Clear();
            if (fipsCompliant)
            {
                Config.TextEncryptionModuleRow row = table.NewTextEncryptionModuleRow();
                row.FileName = "FipsCrypto.dll";
                table.Rows.Add(row);
            }
        }

        public static void Save(this Configuration @this)
        {
            Log.Instance.Debug($"Saving Epi Info configuration: {@this.ConfigFilePath}");
            Configuration.Save(@this);
        }

        public static Configuration Load(string path = null)
        {
            if (path == null)
            {
                path = Configuration.DefaultConfigurationPath;
            }
            Log.Instance.Debug($"Loading Epi Info configuration: {path}");
            Configuration.Load(path);
            return Configuration.GetNewInstance();
        }
    }
}
