using ERHMS.Test.Dapper;
using ERHMS.Utility;
using NUnitLite;

namespace ERHMS.Test
{
    public class Program
    {
        internal static int Main(string[] args)
        {
            TypeMaps.Initialize();
            Settings.Default.Reset();
            return new AutoRun().Execute(args);
        }
    }
}
