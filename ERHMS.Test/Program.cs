using ERHMS.Test.Dapper;
using ERHMS.Utility;
using NUnitLite;

namespace ERHMS.Test
{
    public class Program
    {
        internal static int Main(string[] args)
        {
            Settings.Default.Reset();
            TypeMaps.Initialize();
            return new AutoRun().Execute(args);
        }
    }
}
