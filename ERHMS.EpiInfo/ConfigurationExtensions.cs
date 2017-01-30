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

        private static FileInfo GetConfigurationFile(DirectoryInfo root = null)
        {
            DirectoryInfo directory = (root ?? GetConfigurationRoot()).GetSubdirectory("Configuration");
            return directory.GetFile(Path.GetFileName(Configuration.DefaultConfigurationPath));
        }

        public static Configuration Create()
        {
            FileInfo file = GetConfigurationFile();
            Log.Current.DebugFormat("Creating configuration: {0}", file.FullName);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
            ClearRecents(config);
            ClearConnections(config);
            InitializeDirectories(config, GetConfigurationRoot());
            InitializeSettings(config);
            CopyAssets();
            return new Configuration(file.FullName, config);
        }

        private static void ClearRecents(Config config)
        {
            config.RecentView.Clear();
            config.RecentProject.Clear();
        }

        private static void ClearConnections(Config config)
        {
            config.Connections.Clear();
        }

        private static void InitializeDirectories(Config config, DirectoryInfo root)
        {
            Config.DirectoriesRow directories = config.Directories.Single();
            directories.Archive = root.CreateSubdirectory("Archive").FullName;
            directories.Configuration = root.CreateSubdirectory("Configuration").FullName;
            directories.LogDir = root.CreateSubdirectory("Logs").FullName;
            directories.Output = root.CreateSubdirectory("Output").FullName;
            directories.Project = root.CreateSubdirectory("Projects").FullName;
            directories.Samples = root.CreateSubdirectory(Path.Combine("Resources", "Samples")).FullName;
            DirectoryInfo templates = root.CreateSubdirectory("Templates");
            templates.CreateSubdirectory("Fields");
            templates.CreateSubdirectory("Forms");
            templates.CreateSubdirectory("Pages");
            templates.CreateSubdirectory("Projects");
            directories.Templates = templates.FullName;
            directories.Working = Path.GetTempPath();
        }

        private static void InitializeSettings(Config config)
        {
            if (CryptographyExtensions.RequiresFipsCompliance())
            {
                DataRow row = config.TextEncryptionModule.NewRow();
                row.SetField("FileName", GetApplicationRoot().GetFile("FipsCrypto.dll").FullName);
                config.TextEncryptionModule.Rows.Add(row);
            }
            Config.SettingsRow settings = config.Settings.Single();
            settings.CheckForUpdates = false;
        }

        private static void CopyAssets()
        {
            DirectoryInfo applicationRoot = GetApplicationRoot();
            DirectoryInfo configurationRoot = GetConfigurationRoot();
            CopyAssets(applicationRoot, configurationRoot, "Projects");
            CopyAssets(applicationRoot, configurationRoot, "Resources");
            CopyAssets(applicationRoot, configurationRoot, "Templates");
            DirectoryInfo phin = configurationRoot.GetSubdirectory("Resources", "PHIN");
            if (phin.Exists)
            {
                phin.Delete(true);
            }
            Assembly assembly = Assembly.GetAssembly(typeof(Settings));
            assembly.CopyManifestResourceTo("ERHMS.Utility.LICENSE.txt", configurationRoot.GetFile("LICENSE.txt"));
            assembly.CopyManifestResourceTo("ERHMS.Utility.NOTICE.txt", configurationRoot.GetFile("NOTICE.txt"));
        }

        private static void CopyAssets(DirectoryInfo source, DirectoryInfo target, string subdirectoryName)
        {
            DirectoryInfo subsource = source.GetSubdirectory(subdirectoryName);
            if (subsource.Exists)
            {
                DirectoryInfo subtarget = target.GetSubdirectory(subdirectoryName);
                subsource.CopyTo(subtarget, false);
            }
        }

        public static void Load(string path)
        {
            Log.Current.DebugFormat("Loading configuration: {0}", path);
            Configuration.Load(path);
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
            Log.SetDirectory(new DirectoryInfo(Configuration.GetNewInstance().Directories.LogDir));
            Log.Current.DebugFormat("Loaded configuration: {0}", path);
        }

        public static void Load(FileInfo file)
        {
            Load(file.FullName);
        }

        public static void Load()
        {
            Load(GetConfigurationFile());
        }

        public static bool TryLoad()
        {
            FileInfo file = GetConfigurationFile();
            if (file.Exists)
            {
                Load(file);
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
            Log.Current.DebugFormat("Changing configuration root: {0}", root.FullName);
            GetConfigurationRoot().CopyTo(root, false);
            Config config = (Config)configuration.ConfigDataSet.Copy();
            ClearRecents(config);
            InitializeDirectories(config, root);
            return new Configuration(GetConfigurationFile(root).FullName, config);
        }

        public static void Save(this Configuration @this)
        {
            Log.Current.DebugFormat("Saving configuration: {0}", @this.ConfigFilePath);
            Configuration.Save(@this);
            Configuration.Save(new Configuration(Configuration.DefaultConfigurationPath, @this.ConfigDataSet));
        }

        public static void Load(this Configuration @this)
        {
            Load(@this.ConfigFilePath);
        }
    }
}
