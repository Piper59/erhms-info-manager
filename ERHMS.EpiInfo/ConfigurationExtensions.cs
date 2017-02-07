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
        private static FileInfo GetFile()
        {
            return new FileInfo(Configuration.DefaultConfigurationPath);
        }

        private static DirectoryInfo GetRoot()
        {
            return new DirectoryInfo(Settings.Default.RootDirectory);
        }

        public static Configuration Create()
        {
            FileInfo file = GetFile();
            Log.Current.DebugFormat("Creating configuration: {0}", file.FullName);
            Config config = (Config)Configuration.CreateDefaultConfiguration().ConfigDataSet.Copy();
            ClearRecents(config);
            ClearDatabases(config);
            SetDirectories(config, GetRoot());
            SetSettings(config);
            return new Configuration(file.FullName, config);
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
            directories.Archive = root.GetSubdirectory("Archive").FullName;
            directories.Configuration = GetFile().DirectoryName;
            directories.LogDir = root.GetSubdirectory("Logs").FullName;
            directories.Output = root.GetSubdirectory("Output").FullName;
            directories.Project = root.GetSubdirectory("Projects").FullName;
            directories.Samples = root.GetSubdirectory(Path.Combine("Resources", "Samples")).FullName;
            directories.Templates = root.GetSubdirectory("Templates").FullName;
            directories.Working = Path.GetTempPath();
        }

        private static void SetSettings(Config config)
        {
            if (CryptographyExtensions.RequiresFipsCompliance())
            {
                DataRow row = config.TextEncryptionModule.NewRow();
                row.SetField("FileName", AssemblyExtensions.GetEntryDirectory().GetFile("FipsCrypto.dll").FullName);
                config.TextEncryptionModule.Rows.Add(row);
            }
            Config.SettingsRow settings = config.Settings.Single();
            settings.CheckForUpdates = false;
        }

        public static void Save(this Configuration @this)
        {
            Log.Current.DebugFormat("Saving configuration: {0}", @this.ConfigFilePath);
            Configuration.Save(@this);
        }

        public static Configuration Load(out bool found)
        {
            FileInfo file = GetFile();
            if (file.Exists)
            {
                found = true;
                Log.Current.DebugFormat("Configuration found in default location: {0}", file.FullName);
            }
            else
            {
                if (File.Exists(Settings.Default.ConfigurationFile))
                {
                    found = true;
                    Log.Current.DebugFormat("Configuration found in non-default location: {0}", Settings.Default.ConfigurationFile);
                    Log.Current.DebugFormat("Copying configuration to default location: {0}", file.FullName);
                    File.Copy(Settings.Default.ConfigurationFile, file.FullName);
                }
                else
                {
                    found = false;
                    Log.Current.Debug("Configuration not found");
                    Save(Create());
                }
            }
            if (Settings.Default.ConfigurationFile != file.FullName)
            {
                if (Settings.Default.ConfigurationFile != null)
                {
                    Log.Current.DebugFormat("Unsetting configuration file: {0}", Settings.Default.ConfigurationFile);
                }
                Log.Current.DebugFormat("Setting configuration file: {0}", file.FullName);
                Settings.Default.ConfigurationFile = file.FullName;
                Settings.Default.Save();
            }
            Log.Current.DebugFormat("Loading configuration: {0}", file.FullName);
            Configuration.Load(file.FullName);
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
            Configuration configuration = Configuration.GetNewInstance();
            Log.SetDirectory(new DirectoryInfo(configuration.Directories.LogDir));
            Log.Current.DebugFormat("Loaded configuration: {0}", file.FullName);
            return configuration;
        }

        public static Configuration Load()
        {
            bool found;
            return Load(out found);
        }

        public static void CreateAssets(this Configuration @this)
        {
            DirectoryInfo root = GetRoot();
            Log.Current.DebugFormat("Creating assets: {0}", root.FullName);
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
            DirectoryInfo entryRoot = AssemblyExtensions.GetEntryDirectory();
            CopyToIfExists(entryRoot.GetSubdirectory("Projects"), @this.Directories.Project);
            CopyToIfExists(entryRoot.GetSubdirectory("Templates"), @this.Directories.Templates);
            Assembly assembly = Assembly.GetAssembly(typeof(Settings));
            assembly.CopyManifestResourceTo("ERHMS.Utility.LICENSE.txt", root.GetFile("LICENSE.txt"));
            assembly.CopyManifestResourceTo("ERHMS.Utility.NOTICE.txt", root.GetFile("NOTICE.txt"));
        }

        private static void CopyToIfExists(DirectoryInfo source, string targetPath)
        {
            if (source.Exists)
            {
                source.CopyTo(new DirectoryInfo(targetPath), false);
            }
        }

        public static void ChangeRoot(this Configuration @this, DirectoryInfo root)
        {
            Log.Current.DebugFormat("Changing root: {0}", root.FullName);
            ClearRecents(@this.ConfigDataSet);
            SetDirectories(@this.ConfigDataSet, root);
        }
    }
}
