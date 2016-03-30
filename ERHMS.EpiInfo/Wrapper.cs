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
            string fileName = string.Format("{0}.exe", assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title);
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        protected static Process Execute(Expression<Action<string[]>> expression, params string[] args)
        {
            FileInfo executable = GetExecutable(Assembly.GetCallingAssembly());
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            string _args = Arguments.Format(args.Prepend(methodName));
            Log.Current.DebugFormat("Executing wrapper: {0} {1}", executable.FullName, _args);
            return ProcessExtensions.Start(executable, _args);
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
