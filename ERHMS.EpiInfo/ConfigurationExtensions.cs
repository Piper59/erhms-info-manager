using Epi;
using Epi.DataSets;
using ERHMS.Data;
using System;
using System.IO;
using Settings = ERHMS.EpiInfo.Properties.Settings;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static string GetDatabaseDriver(DatabaseProvider provider)
        {
            switch (provider)
            {
                case DatabaseProvider.Access2003:
                    return Configuration.AccessDriver;
                case DatabaseProvider.Access2007:
                    return Configuration.Access2007Driver;
                case DatabaseProvider.SqlServer:
                    return Configuration.SqlDriver;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }

        public static DatabaseProvider GetDatabaseProvider(string driver)
        {
            switch (driver)
            {
                case Configuration.AccessDriver:
                    return DatabaseProvider.Access2003;
                case Configuration.Access2007Driver:
                    return DatabaseProvider.Access2007;
                case Configuration.SqlDriver:
                    return DatabaseProvider.SqlServer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(driver));
            }
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
            Configuration configuration = Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            configuration.ReadSettings(Settings.Default);
            return configuration;
        }

        public static void Configure(ExecutionEnvironment environment)
        {
            if (!File.Exists(Configuration.DefaultConfigurationPath))
            {
                Configuration.Save(Create());
            }
            Configuration.Load(Configuration.DefaultConfigurationPath);
            Configuration.Environment = environment;
        }
    }
}
