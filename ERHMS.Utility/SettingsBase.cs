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
            serializer = new XmlSerializer(typeof(TSettings));
            Assembly assembly = Assembly.GetAssembly(typeof(TSettings));
            file = new FileInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                assembly.GetCompany().ToPrintable(),
                assembly.GetProduct().ToPrintable(),
                string.Format("{0}.xml", typeof(TSettings).FullName.ToPrintable())));
            Default = Load();
        }

        public static FileInfo GetFile()
        {
            file.Refresh();
            return file;
        }

        public static TSettings Load(out bool loaded)
        {
            try
            {
                using (Stream stream = file.OpenRead())
                {
                    TSettings settings = (TSettings)serializer.Deserialize(stream);
                    loaded = true;
                    return settings;
                }
            }
            catch
            {
                loaded = false;
                return new TSettings();
            }
        }

        public static TSettings Load()
        {
            bool loaded;
            return Load(out loaded);
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
