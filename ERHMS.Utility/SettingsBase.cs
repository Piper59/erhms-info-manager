using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public class SettingsBase<TSettings> where TSettings : new()
    {
        private static FileInfo file;

        public static TSettings Default { get; private set; }

        static SettingsBase()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(TSettings));
            file = new FileInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                assembly.GetCompany(),
                assembly.GetProduct(),
                string.Format("{0}.xml", typeof(TSettings).FullName)));
            try
            {
                Load();
            }
            catch
            {
                Default = new TSettings();
            }
        }

        public static void Reset()
        {
            Default = new TSettings();
        }

        public static void Load()
        {
            if (!file.Exists)
            {
                Default = new TSettings();
            }
            else
            {
                using (Stream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                    Default = (TSettings)serializer.Deserialize(stream);
                }
            }
        }

        public void Save()
        {
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if (!file.Exists)
            {
                using (file.Create()) { }
            }
            using (Stream stream = new FileStream(file.FullName, FileMode.Truncate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                serializer.Serialize(stream, this);
            }
        }
    }
}
