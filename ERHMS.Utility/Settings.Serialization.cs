using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public partial class Settings
    {
        public static readonly string FilePath;

        private static XmlSerializer serializer;

        public static Settings Default { get; private set; }

        static Settings()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Temp";
            string product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? assembly.GetName().Name;
            string fileName = typeof(Settings).FullName + ".xml";
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), company, product, fileName);
            serializer = new XmlSerializer(typeof(Settings));
            Default = Load();
        }

        private static Settings Load()
        {
            try
            {
                using (Stream stream = File.OpenRead(FilePath))
                {
                    return (Settings)serializer.Deserialize(stream);
                }
            }
            catch
            {
                return new Settings();
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream stream = File.Create(FilePath))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
