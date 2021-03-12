using Epi;
using Epi.DataSets;
using ERHMS.Common;
using System.IO;
using Settings = ERHMS.EpiInfo.Properties.Settings;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        private static string FilePath => Configuration.DefaultConfigurationPath;

        public static bool Exists()
        {
            return File.Exists(FilePath);
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

        private static void ReadSettings(this Configuration @this, Settings settings)
        {
            @this.SetTextEncryptionModule(settings.FipsCompliant);
            Config.SettingsRow row = @this.Settings;
            row.ControlFontSize = settings.ControlFontSize;
            row.DefaultPageHeight = settings.DefaultPageHeight;
            row.DefaultPageWidth = settings.DefaultPageWidth;
            row.EditorFontSize = settings.EditorFontSize;
            row.GridSize = settings.GridSize;
        }

        public static Configuration Create()
        {
            Log.Default.Debug($"Creating configuration: {FilePath}");
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            configuration.ReadSettings(Settings.Default);
            return configuration;
        }

        public static void Save(this Configuration @this)
        {
            Log.Default.Debug($"Saving configuration: {@this.ConfigFilePath}");
            Configuration.Save(@this);
        }

        public static Configuration Load()
        {
            Log.Default.Debug($"Loading configuration: {FilePath}");
            Configuration.Load(FilePath);
            return Configuration.GetNewInstance();
        }
    }
}
