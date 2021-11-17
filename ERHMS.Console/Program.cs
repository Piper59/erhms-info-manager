using Epi;
using ERHMS.Common.Logging;
using ERHMS.Console.Utilities;
using System;
using Configuration = ERHMS.EpiInfo.Configuration;

namespace ERHMS.Console
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            IUtility utility = Utility.ParseArgs(args);
            Log.Initialize(new ConsoleAppender());
            Log.Instance.Info("Running");
            try
            {
                Configuration.Initialize(ExecutionEnvironment.Console);
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
    }
}
