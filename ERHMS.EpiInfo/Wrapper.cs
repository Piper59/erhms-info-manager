using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.EpiInfo
{
    public class Wrapper
    {
        protected static FileInfo GetExecutable(Assembly assembly)
        {
            string fileName = string.Format("{0}.exe", assembly.GetTitle());
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        protected static Process Execute(Expression<Action<string[]>> expression, params string[] arguments)
        {
            FileInfo executable = GetExecutable(Assembly.GetCallingAssembly());
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            Process process = ProcessExtensions.Create(executable, arguments.Prepend(methodName));
            Log.Current.DebugFormat("Executing wrapper: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
            process.Start();
            return process;
        }

        protected static void MainBase(Type type, string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Log.Current.Debug("Starting up");
                ConfigurationExtensions.Load();
                ReflectionExtensions.Invoke(type, args.First(), new Type[] { typeof(string[]) }, new object[] { args.Skip(1).ToArray() });
                Log.Current.Debug("Exiting");
            }
            catch (Exception ex)
            {
                Log.Current.Fatal("Fatal error", ex);
                MessageBox.Show("Epi Info encountered an error and must shut down.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected Wrapper() { }
    }
}
