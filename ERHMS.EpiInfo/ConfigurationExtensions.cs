using Epi;
using Epi.DataSets;
using ERHMS.Utility;
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

        private static string GetConfigurationFilePath()
        {
            string fileName = Path.GetFileName(Configuration.DefaultConfigurationPath);
            return Path.Combine(GetConfigurationRoot().FullName, "Configuration", fileName);
        }

        public static Configuration Create()
        {
            string path = GetConfigurationFilePath();
            Log.Current.DebugFormat("Creating configuration: {0}", path);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
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
            Config.DirectoriesRow directories = config.Directories.Single();
            directories.Archive = root.CreateSubdirectory("Archive").FullName;
            directories.Configuration = root.CreateSubdirectory("Configuration").FullName;
            directories.LogDir = root.CreateSubdirectory("Logs").FullName;
            directories.Output = root.CreateSubdirectory("Output").FullName;
            directories.Project = root.CreateSubdirectory("Projects").FullName;
            directories.Samples = root.CreateSubdirectory(Path.Combine("Resources", "Samples")).FullName;
            DeleteIfExists(root.GetSubdirectory(Path.Combine("Resources", "PHIN")));
            DirectoryInfo templates = root.CreateSubdirectory("Templates");
            directories.Templates = templates.FullName;
            templates.CreateSubdirectory("Fields");
            templates.CreateSubdirectory("Forms");
            templates.CreateSubdirectory("Pages");
            templates.CreateSubdirectory("Projects");
            directories.Working = Path.GetTempPath();
        }

        private static void DeleteIfExists(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                return;
            }
            directory.Delete();
        }

        private static void CopyIfExists(DirectoryInfo source, DirectoryInfo target, string subdirectoryName)
        {
            DirectoryInfo subsource = source.GetSubdirectory(subdirectoryName);
            if (!subsource.Exists)
            {
                return;
            }
            DirectoryInfo subtarget = target.GetSubdirectory(subdirectoryName);
            IOExtensions.Copy(subsource, subtarget);
        }

        private static void CopyAssets()
        {
            DirectoryInfo applicationRoot = GetApplicationRoot();
            DirectoryInfo configurationRoot = GetConfigurationRoot();
            CopyIfExists(applicationRoot, configurationRoot, "Projects");
            CopyIfExists(applicationRoot, configurationRoot, "Resources");
            CopyIfExists(applicationRoot, configurationRoot, "Templates");
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
                Configuration.Save(Create());
                Load();
            }
        }

        public static void Refresh(this Configuration @this, bool save)
        {
            if (save)
            {
                Configuration.Save(@this);
            }
            Load(@this.ConfigFilePath);
        }
    }
}
