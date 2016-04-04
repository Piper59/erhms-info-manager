using ERHMS.Utility;
using System;
using System.Collections.Generic;
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
        private static class Arguments
        {
            public static string Escape(string arg)
            {
                return string.Format("\"{0}\"", arg.Replace("\"", "\"\""));
            }

            public static string Format(IEnumerable<string> args)
            {
                return string.Join(" ", args.Select(arg => Escape(arg)));
            }
        }

        protected static FileInfo GetExecutable(Assembly assembly)
        {
            string fileName = string.Format("{0}.exe", assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title);
            return ConfigurationExtensions.GetApplicationRoot().GetFile(fileName);
        }

        protected static Process Execute(Expression<Action<string[]>> expression, params string[] args)
        {
            FileInfo executable = GetExecutable(Assembly.GetCallingAssembly());
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            string argString = Arguments.Format(args.Prepend(methodName));
            Log.Current.DebugFormat("Executing wrapper: {0} {1}", executable.FullName, argString);
            return ProcessExtensions.Start(executable, argString);
        }

        protected static void MainBase(Type type, string[] args)
        {
            Application.EnableVisualStyles();
            Log.Current.Debug("Starting up");
            ConfigurationExtensions.Load();
            ReflectionExtensions.Invoke(type, args.First(), new Type[] { typeof(string[]) }, new object[] { args.Skip(1).ToArray() });
            Log.Current.Debug("Exiting");
        }
    }
}
