using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public class SettingsBase<TSettings> where TSettings : new()
    {
        public static FileInfo File { get; private set; }
        public static TSettings Instance { get; private set; }

        static SettingsBase()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(TSettings));
            File = new FileInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                assembly.GetCompany().ToPrintable(),
                assembly.GetProduct().ToPrintable(),
                string.Format("{0}.xml", typeof(TSettings).FullName.ToPrintable())));
            try
            {
                using (Stream stream = System.IO.File.OpenRead(File.FullName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                    Instance = (TSettings)serializer.Deserialize(stream);
                }
            }
            catch
            {
                Instance = new TSettings();
            }
        }

        protected SettingsBase() { }

        public void Save()
        {
            if (!File.Directory.Exists)
            {
                File.Directory.Create();
            }
            using (Stream stream = System.IO.File.Create(File.FullName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TSettings));
                serializer.Serialize(stream, this);
            }
        }
    }
}
