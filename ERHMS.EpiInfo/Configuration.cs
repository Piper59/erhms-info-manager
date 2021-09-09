using Epi;
using Epi.DataSets;
using ERHMS.Data;
using System;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class Configuration : Epi.Configuration
    {
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
            if (!Exists())
            {
                Save(Create());
            }
            Instance = Load();
            Environment = environment;
        }

        public static bool Exists()
        {
            return File.Exists(DefaultConfigurationPath);
        }

        public static Configuration Create()
        {
            Configuration configuration = new Configuration(CreateDefaultConfiguration());
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            configuration.ReadSettings(Properties.Settings.Default);
            return configuration;
        }

        public static Configuration Load()
        {
            Load(DefaultConfigurationPath);
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
