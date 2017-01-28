using Epi;
using Epi.DataSets;
using ERHMS.Utility;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo
{
    public static class ConfigurationExtensions
    {
        public static DirectoryInfo GetApplicationRoot()
        {
            return new FileInfo(Assembly.GetEntryAssembly().Location).Directory;
        }

        public static DirectoryInfo GetConfigurationRoot()
        {
            return new DirectoryInfo(Settings.Default.RootDirectory);
        }

        private static string GetConfigurationFilePath(DirectoryInfo root = null)
        {
            root = root ?? GetConfigurationRoot();
            string fileName = Path.GetFileName(Configuration.DefaultConfigurationPath);
            return Path.Combine(root.FullName, "Configuration", fileName);
        }

        public static Configuration Create()
        {
            string path = GetConfigurationFilePath();
            Log.Current.DebugFormat("Creating configuration: {0}", path);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
            if (CryptographyExtensions.RequiresFipsCompliance())
            {
                DataRow row = config.TextEncryptionModule.NewRow();
                row.SetField("FileName", GetApplicationRoot().GetFile("FipsCrypto.dll").FullName);
                config.TextEncryptionModule.Rows.Add(row);
            }
            InitializeDirectories(config);
            CopyAssets();
            config.RecentView.Clear();
            config.RecentProject.Clear();
            config.Database.Clear();
            config.File.Clear();
            InitializeSettings(config);
            return new Configuration(path, config);
        }

        private static void InitializeDirectories(Config config)
        {
            DirectoryInfo root = GetConfigurationRoot();
            SetDirectories(config, root);
            DirectoryInfo phin = root.GetSubdirectory("Resources", "PHIN");
            if (phin.Exists)
            {
                phin.Delete();
            }
            DirectoryInfo templates = new DirectoryInfo(config.Directories.Single().Templates);
            templates.CreateSubdirectory("Fields");
            templates.CreateSubdirectory("Forms");
            templates.CreateSubdirectory("Pages");
            templates.CreateSubdirectory("Projects");
        }

        private static void SetDirectories(Config config, DirectoryInfo root)
        {
            Config.DirectoriesRow directories = config.Directories.Single();
            directories.Archive = root.CreateSubdirectory("Archive").FullName;
            directories.Configuration = root.CreateSubdirectory("Configuration").FullName;
            directories.LogDir = root.CreateSubdirectory("Logs").FullName;
            directories.Output = root.CreateSubdirectory("Output").FullName;
            directories.Project = root.CreateSubdirectory("Projects").FullName;
            directories.Samples = root.CreateSubdirectory(Path.Combine("Resources", "Samples")).FullName;
            directories.Templates = root.CreateSubdirectory("Templates").FullName;
            directories.Working = Path.GetTempPath();
        }

        private static void CopyIfExists(DirectoryInfo source, DirectoryInfo target, string subdirectoryName)
        {
            DirectoryInfo subsource = source.GetSubdirectory(subdirectoryName);
            if (subsource.Exists)
            {
                DirectoryInfo subtarget = target.GetSubdirectory(subdirectoryName);
                subsource.CopyTo(subtarget, false);
            }
        }

        private static void CopyAssets()
        {
            DirectoryInfo applicationRoot = GetApplicationRoot();
            DirectoryInfo configurationRoot = GetConfigurationRoot();
            CopyIfExists(applicationRoot, configurationRoot, "Projects");
            CopyIfExists(applicationRoot, configurationRoot, "Resources");
            CopyIfExists(applicationRoot, configurationRoot, "Templates");
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.EpiInfo.LICENSE.txt", configurationRoot.GetFile("LICENSE.txt"));
            assembly.CopyManifestResourceTo("ERHMS.EpiInfo.NOTICE.txt", configurationRoot.GetFile("NOTICE.txt"));
        }

        private static void InitializeSettings(Config config)
        {
            Config.SettingsRow settings = config.Settings.Single();
            settings.CheckForUpdates = false;
        }

        public static void Load(string path)
        {
            Log.Current.DebugFormat("Loading configuration: {0}", path);
            Configuration.Load(path);
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
            Log.Configure(Configuration.GetNewInstance());
            Log.Current.DebugFormat("Loaded configuration: {0}", path);
        }

        public static void Load()
        {
            Load(GetConfigurationFilePath());
        }

        public static bool TryLoad()
        {
            string path = GetConfigurationFilePath();
            if (File.Exists(path))
            {
                Load(path);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CreateAndOrLoad()
        {
            if (!TryLoad())
            {
                Save(Create());
                Load();
            }
        }

        public static Configuration ChangeRoot(Configuration configuration, DirectoryInfo root)
        {
            GetApplicationRoot().CopyTo(root, false);
            Config config = (Config)configuration.ConfigDataSet.Copy();
            SetDirectories(config, root);
            config.RecentView.Clear();
            config.RecentProject.Clear();
            return new Configuration(GetConfigurationFilePath(root), config);
        }

        public static void Save(this Configuration @this)
        {
            Configuration.Save(@this);
            FileInfo defaultConfigurationFile = new FileInfo(Configuration.DefaultConfigurationPath);
            defaultConfigurationFile.Directory.Create();
            File.Copy(@this.ConfigFilePath, defaultConfigurationFile.FullName, true);
        }

        public static void Refresh(this Configuration @this, bool save)
        {
            if (save)
            {
                @this.Save();
            }
            Load(@this.ConfigFilePath);
        }
    }
}
