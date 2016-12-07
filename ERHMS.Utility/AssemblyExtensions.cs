using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class AssemblyExtensions
    {
        public static string GetCompany(this Assembly @this)
        {
            return @this.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
        }

        public static string GetProduct(this Assembly @this)
        {
            return @this.GetCustomAttribute<AssemblyProductAttribute>().Product;
        }

        public static string GetTitle(this Assembly @this)
        {
            return @this.GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        public static string GetVersion(this Assembly @this)
        {
            return @this.GetName().Version.ToString();
        }

        public static string GetInformationalVersion(this Assembly @this)
        {
            return @this.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static string GetManifestResourceText(this Assembly @this, string resourceName)
        {
            using (Stream stream = @this.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void SaveResource(this Assembly @this, string resourceName, string path)
        {
            using (Stream source = @this.GetManifestResourceStream(resourceName))
            using (Stream target = File.Create(path))
            {
                source.CopyTo(target);
            }
        }
    }
}
