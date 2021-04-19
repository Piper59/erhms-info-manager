using Epi;
using ERHMS.Common;
using ERHMS.Console.Utilities;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console
{
    public class Program
    {
        private static int Main(string[] args)
        {
            IUtility utility = Utility.ParseArgs(args);
            Log.Configure(Log.Appenders.Console);
            Log.Instance.Info("Running");
            try
            {
                ConfigureEpiInfo();
                utility.Run();
                Log.Instance.Info("Completed");
                return ErrorCodes.Success;
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex.Message);
                Log.Instance.Error(ex);
                Log.Instance.Warn("Completed with errors");
                return ex.HResult;
            }
        }

        private static void ConfigureEpiInfo()
        {
            if (!ConfigurationExtensions.Exists())
            {
                Configuration configuration = ConfigurationExtensions.Create();
                configuration.Save();
            }
            ConfigurationExtensions.Load();
            Configuration.Environment = ExecutionEnvironment.Console;
        }
    }
}
