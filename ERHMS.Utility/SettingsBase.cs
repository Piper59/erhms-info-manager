using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ERHMS.Utility
{
    public class SettingsBase<TSettings> where TSettings : new()
    {
        private static readonly Regex NonAscii = new Regex(@"[^\u0020-\u007e]");

        private static FileInfo file;

        public static TSettings Default { get; private set; }

        private static string ToAscii(string value)
        {
            return NonAscii.Replace(value, "");
        }

        static SettingsBase()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(TSettings));
            file = new FileInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ToAscii(assembly.GetCompany()),
                ToAscii(assembly.GetProduct()),
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
