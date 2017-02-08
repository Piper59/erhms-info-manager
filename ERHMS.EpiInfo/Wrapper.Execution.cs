using ERHMS.Utility;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ERHMS.EpiInfo
{
    public partial class Wrapper
    {
        protected static TextReader In { get; private set; }
        protected static TextWriter Out { get; private set; }

        protected static void MainBase(Type type, string[] args)
        {
            try
            {
                Log.Current.Debug("Starting up");
                Application.ThreadException += (sender, e) =>
                {
                    HandleError(e.Exception);
                };
                Application.EnableVisualStyles();
                ConfigurationExtensions.Load();
                In = Console.In;
                Out = Console.Out;
                Console.SetIn(new StreamReader(Stream.Null));
                Console.SetOut(new StreamWriter(Stream.Null));
                ReflectionExtensions.Invoke(type, args[0], new Type[] { typeof(string[]) }, new object[] { args.Skip(1).ToArray() });
                Log.Current.Debug("Exiting");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        protected static void HandleError(Exception ex)
        {
            Log.Current.Fatal("Fatal error", ex);
            MessageBox.Show("Epi Info\u2122 encountered an error and must shut down.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // TODO: Define convenience methods for raising events
    }
}
