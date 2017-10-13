using ERHMS.Utility;
using NUnitLite;
using System;
using System.Reflection;
using System.IO;

namespace ERHMS.Test
{
    public class Program
    {
        internal static int Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            TypeMaps.Configure();
            Settings.Default.Reset();
            return new AutoRun().Execute(args);
        }
    }
}
