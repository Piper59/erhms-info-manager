using Epi;
using Epi.DataSets;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static readonly string FilePath = Configuration.DefaultConfigurationPath;
        private static readonly ICollection<string> TemplateSubdirectoryNames = new string[]
        {
            "Fields",
            "Forms",
            "Pages",
            "Projects"
        };

        public static string EncryptSafe(string value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return Configuration.Encrypt(value);
            }
        }

        public static string DecryptSafe(string value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return Configuration.Decrypt(value);
            }
        }

        public static Configuration Create(string userDirectoryPath)
        {
            Log.Logger.DebugFormat("Creating configuration: {0}", userDirectoryPath);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
            ClearRecents(config);
            ClearDatabases(config);
            SetDirectories(config, userDirectoryPath);
            SetSettings(config);
            return new Configuration(FilePath, config);
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

        private static void SetDirectories(Config config, string userDirectoryPath)
        {
            Config.DirectoriesRow directories = config.Directories[0];
            SetSystemDirectories(directories);
            SetUserDirectories(directories, userDirectoryPath);
        }

        private static void SetSystemDirectories(Config.DirectoriesRow directories)
        {
            directories.Configuration = Path.GetDirectoryName(FilePath);
            directories.LogDir = Path.GetDirectoryName(Log.FilePath);
            directories.Working = Path.GetTempPath();
        }

        private static void SetUserDirectories(Config.DirectoriesRow directories, string userDirectoryPath)
        {
            directories.Project = Path.Combine(userDirectoryPath, "Projects");
            directories.Templates = Path.Combine(userDirectoryPath, "Templates");
        }

        private static void SetSettings(Config config)
        {
            SetFipsCrypto(config, CryptographyExtensions.IsFipsCryptoRequired());
            Config.SettingsRow settings = config.Settings.Single();
            settings.CheckForUpdates = false;
        }

        private static void SetFipsCrypto(Config config, bool required)
        {
            config.TextEncryptionModule.Clear();
            if (required)
            {
                config.TextEncryptionModule.AddTextEncryptionModuleRow(null, null, "FipsCrypto.dll");
            }
        }

        public static Configuration Load()
        {
            Log.Logger.DebugFormat("Loading configuration: {0}", FilePath);
            Configuration.Load(FilePath);
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
            return Configuration.GetNewInstance();
        }

        public static bool TryLoad(out Configuration configuration)
        {
            if (File.Exists(FilePath))
            {
                configuration = Load();
                return true;
            }
            else
            {
                configuration = null;
                return false;
            }
        }

        public static void SetFipsCrypto(this Configuration @this, bool required)
        {
            SetFipsCrypto(@this.ConfigDataSet, required);
        }

        public static void Save(this Configuration @this)
        {
            Log.Logger.DebugFormat("Saving configuration: {0}", @this.ConfigFilePath);
            Configuration.Save(@this);
        }

        public static void CreateUserDirectories(this Configuration @this)
        {
            Log.Logger.Debug("Creating user directories");
            Directory.CreateDirectory(@this.Directories.Project);
            DirectoryInfo templates = Directory.CreateDirectory(@this.Directories.Templates);
            foreach (string name in TemplateSubdirectoryNames)
            {
                templates.CreateSubdirectory(name);
            }
        }

        public static void SetUserDirectories(this Configuration @this, string userDirectoryPath)
        {
            Log.Logger.DebugFormat("Setting user directories: {0}", userDirectoryPath);
            SetUserDirectories(@this.ConfigDataSet.Directories[0], userDirectoryPath);
        }
    }
}
