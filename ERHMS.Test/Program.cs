using ERHMS.Utility;
using NUnitLite;

namespace ERHMS.Test
{
    public class Program
    {
        internal static int Main(string[] args)
        {
            TypeMaps.Configure();
            Settings.Default.Reset();
            return new AutoRun().Execute(args);
        }
    }
}
