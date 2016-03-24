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
        protected static FileInfo GetExecutable(Module module)
        {
            string fileName = string.Format("ERHMS.EpiInfo.{0}.exe", module);
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        protected static Process Execute(Module module, Expression<Action<string[]>> expression, params string[] args)
        {
            Log.Current.DebugFormat("Executing wrapper: {0} {1}", module, args);
            FileInfo executable = GetExecutable(module);
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            return ProcessExtensions.Start(executable, ModuleExtensions.Arguments.Format(args.Prepend(methodName)));
        }

        protected static void MainBase(Type type, string[] args)
        {
            Application.EnableVisualStyles();
            Log.Current.Debug("Starting up");
            ConfigurationExtensions.Load();
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo method = type.GetMethod(args.First(), bindingFlags, null, new Type[] { typeof(string[]) }, null);
            method.Invoke(null, new object[] { args.Skip(1).ToArray() });
            Log.Current.Debug("Exiting");
        }
    }
}
