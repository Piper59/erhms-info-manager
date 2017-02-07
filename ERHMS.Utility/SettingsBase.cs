using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public class SettingsBase<TSettings> where TSettings : new()
    {
        private static FileInfo file;
        private static XmlSerializer serializer;

        public static TSettings Default { get; private set; }

        static SettingsBase()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(TSettings));
            file = new FileInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                assembly.GetCompany().ToPrintable(),
                assembly.GetProduct().ToPrintable(),
                string.Format("{0}.xml", typeof(TSettings).FullName.ToPrintable())));
            serializer = new XmlSerializer(typeof(TSettings));
            Default = Load();
        }

        public static FileInfo GetFile()
        {
            return new FileInfo(file.FullName);
        }

        public static TSettings Load(out bool found)
        {
            try
            {
                using (Stream stream = file.OpenRead())
                {
                    TSettings settings = (TSettings)serializer.Deserialize(stream);
                    found = true;
                    return settings;
                }
            }
            catch
            {
                found = false;
                return new TSettings();
            }
        }

        public static TSettings Load()
        {
            bool found;
            return Load(out found);
        }

        protected SettingsBase() { }

        public void Save()
        {
            file.Directory.Create();
            using (Stream stream = file.Create())
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
