using System.IO;
using System.Reflection;

namespace ERHMS.Desktop.ViewModels
{
    public class AboutViewModel
    {
        public string Product { get; }
        public string Version { get; }
        public string Copyright { get; }
        public string Notice { get; }

        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Product = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            Version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            using (Stream stream = assembly.GetManifestResourceStream("ERHMS.Desktop.Resources.NOTICE.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                Notice = reader.ReadToEnd();
            }
        }
    }
}
