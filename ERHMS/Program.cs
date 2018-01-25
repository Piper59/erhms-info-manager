using System;

namespace ERHMS
{
    public class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Launcher.Launcher.Execute(args);
        }
    }
}
