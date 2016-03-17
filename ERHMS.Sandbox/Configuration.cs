using System.Configuration;

namespace ERHMS.Sandbox
{
    public static class Configuration
    {
        public static string ServiceAddress
        {
            get { return ConfigurationManager.AppSettings["ServiceAddress"]; }
        }
    }
}
