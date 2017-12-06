using ERHMS.DataAccess;
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
            Settings.Default.Reset();
            TypeMaps.Configure();
            DataContext.Configure();
            AutoRun app = new AutoRun();
            return app.Execute(args);
        }
    }
}
