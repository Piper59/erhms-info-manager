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
    public static class WrapperBase
    {
        private static string EscapeArg(string arg)
        {
            return string.Format("\"{0}\"", arg.Replace("\"", "\"\""));
        }

        private static string FormatArgs(IEnumerable<string> args)
        {
            return string.Join(" ", args.Select(arg => EscapeArg(arg ?? "")));
        }

        public static Process Execute(Expression<Action<string[]>> expression, params string[] args)
        {
            string fileName = string.Format("{0}.exe", Assembly.GetCallingAssembly().GetName().Name);
            FileInfo executable = AssemblyExtensions.GetEntryDirectory().GetFile(fileName);
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = executable.DirectoryName;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = executable.FullName;
            process.StartInfo.Arguments = FormatArgs(args.Prepend(methodName));
            Log.Current.DebugFormat("Executing wrapper: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
            process.Start();
            return process;
        }

        public static void MainBase(Type type, string[] args)
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
                ReflectionExtensions.Invoke(type, args[0], new Type[] { typeof(string[]) }, args.Skip(1).ToArray());
                Log.Current.Debug("Exiting");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void HandleError(Exception ex)
        {
            Log.Current.Fatal("Fatal error", ex);
            MessageBox.Show("Epi Info\u2122 encountered an error and must shut down.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
