using Epi;
using Epi.DataSets;
using ERHMS.Utility;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static Configuration Create(DirectoryInfo root)
        {
            Log.Current.DebugFormat("Creating configuration: {0}", Configuration.DefaultConfigurationPath);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
            ClearRecents(config);
            ClearDatabases(config);
            SetDirectories(config, root);
            SetSettings(config);
            return new Configuration(Configuration.DefaultConfigurationPath, config);
        }

        private static void ClearRecents(Config config)
        {
            config.RecentView.Clear();
            config.RecentProject.Clear();
        }

        private static void ClearDatabases(Config config)
        {
            config.Database.Clear();
        }

        private static void SetDirectories(Config config, DirectoryInfo root)
        {
            Config.DirectoriesRow directories = config.Directories.Single();
            directories.Archive = root.GetDirectory("Archive").FullName;
            directories.Configuration = Path.GetDirectoryName(Configuration.DefaultConfigurationPath);
            directories.LogDir = root.GetDirectory("Logs").FullName;
            directories.Output = root.GetDirectory("Output").FullName;
            directories.Project = root.GetDirectory("Projects").FullName;
            directories.Samples = root.GetDirectory(Path.Combine("Resources", "Samples")).FullName;
            directories.Templates = root.GetDirectory("Templates").FullName;
            directories.Working = Path.GetTempPath();
        }

        private static void SetSettings(Config config)
        {
            if (RequiresFipsCrypto())
            {
                DataRow row = config.TextEncryptionModule.NewRow();
                row.SetField("FileName", AssemblyExtensions.GetEntryDirectory().GetFile("FipsCrypto.dll").FullName);
                config.TextEncryptionModule.Rows.Add(row);
            }
            Config.SettingsRow settings = config.Settings.Single();
            settings.CheckForUpdates = false;
        }

        private static bool RequiresFipsCrypto()
        {
            try
            {
                new MD5CryptoServiceProvider();
                return false;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        public static Configuration Load(string path)
        {
            Log.Current.DebugFormat("Loading configuration: {0}", path);
            Configuration.Load(path);
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
            Configuration configuration = Configuration.GetNewInstance();
            Log.SetDirectory(new DirectoryInfo(configuration.Directories.LogDir));
            Log.Current.DebugFormat("Loaded configuration: {0}", path);
            return configuration;
        }

        public static Configuration Load()
        {
            return Load(Configuration.DefaultConfigurationPath);
        }

        public static bool TryLoad(string path, out Configuration configuration)
        {
            Log.Current.DebugFormat("Trying to load configuration: {0}", path);
            try
            {
                if (File.Exists(path))
                {
                    configuration = Load(path);
                    return true;
                }
                else
                {
                    Log.Current.Debug("Configuration not found");
                }
            }
            catch (Exception ex)
            {
                Log.Current.Warn("Failed to load configuration", ex);
            }
            configuration = null;
            return false;
        }

        public static bool TryLoad(out Configuration configuration)
        {
            return TryLoad(Configuration.DefaultConfigurationPath, out configuration);
        }

        public static void Save(this Configuration @this)
        {
            Log.Current.DebugFormat("Saving configuration: {0}", @this.ConfigFilePath);
            Configuration.Save(@this);
        }

        public static void CreateDirectories(this Configuration @this)
        {
            Directory.CreateDirectory(@this.Directories.Archive);
            Directory.CreateDirectory(@this.Directories.LogDir);
            Directory.CreateDirectory(@this.Directories.Output);
            Directory.CreateDirectory(@this.Directories.Project);
            Directory.CreateDirectory(@this.Directories.Samples);
            DirectoryInfo templates = Directory.CreateDirectory(@this.Directories.Templates);
            templates.CreateSubdirectory("Fields");
            templates.CreateSubdirectory("Forms");
            templates.CreateSubdirectory("Pages");
            templates.CreateSubdirectory("Projects");
        }

        public static void ChangeRoot(this Configuration @this, DirectoryInfo root)
        {
            Log.Current.DebugFormat("Changing root: {0}", root.FullName);
            ClearRecents(@this.ConfigDataSet);
            SetDirectories(@this.ConfigDataSet, root);
        }
    }
}
