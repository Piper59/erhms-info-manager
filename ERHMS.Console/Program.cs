using Epi;
using ERHMS.Console.Utilities;
using ERHMS.EpiInfo;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERHMS.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ConfigureLog();
            try
            {
                ConfigureEpiInfo();
                GetUtility(args).Run();
            }
            catch (Exception ex)
            {
                Log.Default.Fatal(ex.Message);
                Log.Default.Debug(ex.StackTrace);
            }
        }

        private static void ConfigureLog()
        {
            XmlConfigurator.Configure();
        }

        private static void ConfigureEpiInfo()
        {
            if (ConfigurationExtensions.Exists())
            {
                ConfigurationExtensions.Load();
            }
            else
            {
                string path = Path.GetTempFileName();
                Configuration configuration = ConfigurationExtensions.Create(path);
                configuration.Save();
                ConfigurationExtensions.Load(path);
            }
            Configuration.Environment = ExecutionEnvironment.Console;
        }

        private static IUtility GetUtility(IList<string> args)
        {
            if (args.Count == 0)
            {
                return new Help
                {
                    Writer = System.Console.Error
                };
            }
            return Utility.Create(args[0], args.Skip(1).ToList());
        }
    }
}
