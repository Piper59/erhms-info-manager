using Epi;
using Epi.DataSets;
using ERHMS.Common.IO;
using ERHMS.Data;
using System;
using System.IO;
using Settings = ERHMS.EpiInfo.Properties.Settings;

namespace ERHMS
{
    public static class Configuration
    {
        public static string FilePath => Epi.Configuration.DefaultConfigurationPath;
        public static Epi.Configuration Instance { get; private set; }

        public static string GetDatabaseDriver(DatabaseProvider provider)
        {
            switch (provider)
            {
                case DatabaseProvider.Access2003:
                    return Epi.Configuration.AccessDriver;
                case DatabaseProvider.Access2007:
                    return Epi.Configuration.Access2007Driver;
                case DatabaseProvider.SqlServer:
                    return Epi.Configuration.SqlDriver;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }

        public static DatabaseProvider GetDatabaseProvider(string driver)
        {
            switch (driver)
            {
                case Epi.Configuration.AccessDriver:
                    return DatabaseProvider.Access2003;
                case Epi.Configuration.Access2007Driver:
                    return DatabaseProvider.Access2007;
                case Epi.Configuration.SqlDriver:
                    return DatabaseProvider.SqlServer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(driver));
            }
        }

        public static string GetProjectsDirectory(this Epi.Configuration @this)
        {
            return PathExtensions.TrimEnd(@this.Directories.Project);
        }

        public static string GetTemplatesDirectory(this Epi.Configuration @this)
        {
            return PathExtensions.TrimEnd(@this.Directories.Templates);
        }

        public static void SetTextEncryptionModule(this Epi.Configuration @this, bool fipsCompliant)
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

        private static void ReadSettings(this Epi.Configuration @this, Settings settings)
        {
            @this.SetTextEncryptionModule(settings.FipsCompliant);
            Config.SettingsRow row = @this.Settings;
            row.ControlFontSize = settings.ControlFontSize;
            row.DefaultPageHeight = settings.DefaultPageHeight;
            row.DefaultPageWidth = settings.DefaultPageWidth;
            row.EditorFontSize = settings.EditorFontSize;
            row.GridSize = settings.GridSize;
        }

        public static Epi.Configuration Create()
        {
            Epi.Configuration configuration = Epi.Configuration.CreateDefaultConfiguration();
            configuration.RecentViews.Clear();
            configuration.RecentProjects.Clear();
            configuration.ReadSettings(Settings.Default);
            return configuration;
        }

        public static void Initialize(ExecutionEnvironment environment)
        {
            if (!File.Exists(FilePath))
            {
                Epi.Configuration.Save(Create());
            }
            Epi.Configuration.Load(FilePath);
            Instance = Epi.Configuration.GetNewInstance();
            Epi.Configuration.Environment = environment;
        }
    }
}
