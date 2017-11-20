using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public partial class Settings
    {
        public static readonly string FilePath;

        public static Settings Default { get; private set; }

        static Settings()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Temp";
            string product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? assembly.GetName().Name;
            string fileName = typeof(Settings).FullName + ".xml";
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), company, product, fileName);
            Load();
        }

        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(Settings));
        }

        private static void Load()
        {
            try
            {
                using (Stream stream = File.OpenRead(FilePath))
                {
                    Default = (Settings)GetSerializer().Deserialize(stream);
                }
            }
            catch
            {
                Default = new Settings();
                Default.Reset();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                using (Stream stream = File.Create(FilePath))
                {
                    GetSerializer().Serialize(stream, this);
                }
            }
            catch { }
        }
    }
}
