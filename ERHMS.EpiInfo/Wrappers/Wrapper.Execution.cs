using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    public partial class Wrapper
    {
        protected static TextReader In { get; private set; }
        protected static TextWriter Out { get; private set; }
        private static TextWriter Error { get; set; }

        protected static void MainBase(Type type, string[] args)
        {
            try
            {
                Log.Logger.Debug("Starting up");
                Application.ThreadException += (sender, e) =>
                {
                    HandleError(e.Exception);
                };
                Application.EnableVisualStyles();
                ConfigurationExtensions.Load();
                In = Console.In;
                Out = Console.Out;
                Error = Console.Error;
                Console.SetIn(new StreamReader(Stream.Null));
                Console.SetOut(new StreamWriter(Stream.Null));
                Console.SetError(new StreamWriter(Stream.Null));
                MethodInfo method = type.GetMethod(args[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                method.Invoke(null, ReceiveArgs());
                Log.Logger.Debug("Exiting");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static object[] ReceiveArgs()
        {
            int count = int.Parse(In.ReadLine());
            if (count == 0)
            {
                return null;
            }
            else
            {
                object[] args = new object[count];
                for (int index = 0; index < count; index++)
                {
                    string line = In.ReadLine();
                    args[index] = line == "" ? null : ConvertExtensions.FromBase64String(line);
                }
                return args;
            }
        }

        protected static void HandleError(Exception ex)
        {
            Log.Logger.Fatal("Fatal error", ex);
            MessageBox.Show("Epi Info\u2122 encountered an error and must shut down.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected static void RaiseEvent(WrapperEventType type, object properties = null)
        {
            Log.Logger.DebugFormat("Raising event: {0}", type);
            Error.WriteLine(WrapperEventArgs.GetData(type, properties));
        }
    }
}
