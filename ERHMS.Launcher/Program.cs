using System;
using System.IO;

namespace ERHMS.Launcher
{
    public class Program
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Launcher.Execute(Path.Combine(Launcher.GetEntryDirectoryPath(), "ERHMS Info Manager"), args);
        }
    }
}
