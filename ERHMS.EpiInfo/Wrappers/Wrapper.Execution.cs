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

        public static void MainBase(string[] args)
        {
            try
            {
                SetStreams();
                Log.LevelName = Settings.Default.LogLevelName;
                Log.Logger.Debug("Starting up");
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    HandleError(e.ExceptionObject as Exception);
                };
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
                Application.EnableVisualStyles();
                ConfigurationExtensions.Load();
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                MethodInfo method = Assembly.GetCallingAssembly().GetType(args[0]).GetMethod(args[1], flags);
                method.Invoke(null, ReceiveArgs());
                Log.Logger.Debug("Exiting");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void SetStreams()
        {
            In = Console.In;
            Out = Console.Out;
            Error = Console.Error;
            Console.SetIn(new StreamReader(Stream.Null));
            Console.SetOut(new StreamWriter(Stream.Null));
            Console.SetError(new StreamWriter(Stream.Null));
        }

        protected static void HandleError(Exception ex)
        {
            Log.Logger.Fatal("Fatal error", ex);
            MessageBox.Show("Epi Info\u2122 encountered an error and must shut down.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static object[] ReceiveArgs()
        {
            Log.Logger.Debug("Receiving args");
            int count = int.Parse(In.ReadLine());
            object[] args = new object[count];
            for (int index = 0; index < count; index++)
            {
                args[index] = Base64Extensions.FromBase64String(In.ReadLine());
            }
            return args;
        }

        protected static void RaiseEvent(string type, object properties = null)
        {
            Log.Logger.DebugFormat("Raising event: {0}", type);
            Error.WriteLine(WrapperEventArgs.Serialize(type, properties));
        }
    }
}
