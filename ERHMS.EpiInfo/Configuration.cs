using Epi;
using Epi.DataSets;
using ERHMS.Common.Logging;
using ERHMS.Data;
using System;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class Configuration : Epi.Configuration
    {
        private static string FilePath => DefaultConfigurationPath;
        public static Configuration Instance { get; private set; }

        public static string GetDatabaseDriver(DatabaseProvider provider)
        {
            switch (provider)
            {
                case DatabaseProvider.Access2003:
                    return AccessDriver;
                case DatabaseProvider.Access2007:
                    return Access2007Driver;
                case DatabaseProvider.SqlServer:
                    return SqlDriver;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }

        public static DatabaseProvider GetDatabaseProvider(string driver)
        {
            switch (driver)
            {
                case AccessDriver:
                    return DatabaseProvider.Access2003;
                case Access2007Driver:
                    return DatabaseProvider.Access2007;
                case SqlDriver:
                    return DatabaseProvider.SqlServer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(driver));
            }
        }

        public static void Initialize(ExecutionEnvironment environment)
        {
            Log.Instance.Debug("Configuring Epi Info");
            try
            {
                if (!File.Exists(FilePath))
                {
                    Configuration configuration = Create();
                    configuration.Save();
                }
                Instance = Load();
                Environment = environment;
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Epi Info could not be configured from {FilePath}.", ex);
            }
        }

        private static Configuration Create()
        {
            Configuration configuration = new Configuration(CreateDefaultConfiguration());
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            configuration.ReadSettings(Properties.Settings.Default);
            return configuration;
        }

        private static Configuration Load()
        {
            Load(FilePath);
            return new Configuration(GetNewInstance());
        }

        public new Directories Directories { get; }

        private Configuration(Epi.Configuration configuration)
            : base(configuration.ConfigFilePath, configuration.ConfigDataSet)
        {
            Directories = new Directories(this);
        }

        private void ReadSettings(Properties.Settings settings)
        {
            SetTextEncryptionModule(settings.FipsCompliant);
            Config.SettingsRow row = Settings;
            row.ControlFontSize = settings.ControlFontSize;
            row.DefaultPageHeight = settings.DefaultPageHeight;
            row.DefaultPageWidth = settings.DefaultPageWidth;
            row.EditorFontSize = settings.EditorFontSize;
            row.GridSize = settings.GridSize;
        }

        public void SetTextEncryptionModule(bool fipsCompliant)
        {
            Config.TextEncryptionModuleDataTable table = ConfigDataSet.TextEncryptionModule;
            table.Clear();
            if (fipsCompliant)
            {
                Config.TextEncryptionModuleRow row = table.NewTextEncryptionModuleRow();
                row.FileName = "FipsCrypto.dll";
                table.Rows.Add(row);
            }
        }

        public void Save()
        {
            Save(this);
        }
    }
}
