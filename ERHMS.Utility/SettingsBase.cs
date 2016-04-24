using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public class SettingsBase<TSettings> where TSettings : new()
    {
        private static FileInfo file;

        public static TSettings Instance { get; private set; }

        static SettingsBase()
        {
            Assembly assembly = typeof(TSettings).Assembly;
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
                Instance = new TSettings();
            }
        }

        public static void Load()
        {
            if (!file.Exists)
            {
                Instance = new TSettings();
            }
            else
            {
                using (Stream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                    Instance = (TSettings)serializer.Deserialize(stream);
                }
            }
        }

        public void Save()
        {
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            using (Stream stream = new FileStream(file.FullName, FileMode.Truncate, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                serializer.Serialize(stream, this);
            }
        }
    }
}
