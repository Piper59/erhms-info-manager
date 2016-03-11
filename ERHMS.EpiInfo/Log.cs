using Epi;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.EpiInfo
{
    public static class Log
    {
        public static string Name
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        public static ILog Current
        {
            get { return LogManager.GetLogger(Name); }
        }

        static Log()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                string message = "Fatal error";
                Exception ex = e.ExceptionObject as Exception;
                if (ex == null)
                {
                    Current.Fatal(message);
                }
                else
                {
                    Current.Fatal(message, ex);
                }
            };
        }

        public static void Configure(Configuration configuration)
        {
            string fileName = string.Format("{0}.txt", Name);
            GlobalContext.Properties["LogPath"] = Path.Combine(configuration.Directories.LogDir, fileName);
            XmlConfigurator.Configure();
        }
    }
}
