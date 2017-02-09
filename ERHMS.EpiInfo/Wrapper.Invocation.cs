using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ERHMS.EpiInfo
{
    public partial class Wrapper
    {
        protected static Wrapper Create(Expression<Action<string[]>> expression, params string[] args)
        {
            return new Wrapper(Assembly.GetCallingAssembly(), expression, args);
        }

        private FileInfo executable;
        private string arguments;
        private Process process;

        public ManualResetEvent Exited { get; private set; }

        protected Wrapper()
        {
            throw new NotSupportedException();
        }

        private Wrapper(Assembly assembly, Expression<Action<string[]>> expression, params string[] args)
        {
            string fileName = string.Format("{0}.exe", assembly.GetName().Name);
            executable = AssemblyExtensions.GetEntryDirectory().GetFile(fileName);
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            arguments = ProcessExtensions.FormatArgs(args.Prepend(methodName));
            Exited = new ManualResetEvent(false);
        }

        public event EventHandler<WrapperEventArgs> Event;
        private void OnEvent(WrapperEventArgs e)
        {
            Event?.Invoke(this, e);
        }
        private void OnEvent(string line)
        {
            OnEvent(WrapperEventArgs.Parse(line));
        }

        public void Invoke()
        {
            Log.Current.DebugFormat("Invoking wrapper: {0} {1}", executable.FullName, arguments);
            process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = executable.DirectoryName,
                    FileName = executable.FullName,
                    Arguments = arguments
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Log.Current.DebugFormat("Received data from wrapper {0}: {1}", process.Id, e.Data);
                    OnEvent(e.Data);
                }
            };
            process.Exited += (sender, e) =>
            {
                Log.Current.DebugFormat("Wrapper exited: {0}", process.Id);
                Exited.Set();
            };
            process.Start();
            Log.Current.DebugFormat("Wrapper invoked: {0}", process.Id);
            process.BeginErrorReadLine();
        }

        public string ReadLine()
        {
            return process.StandardOutput.ReadLine();
        }

        public string ReadToEnd()
        {
            return process.StandardOutput.ReadToEnd();
        }

        public void WriteLine(object value)
        {
            process.StandardInput.WriteLine(value);
        }

        public void WriteLine(string value)
        {
            process.StandardInput.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args)
        {
            process.StandardInput.WriteLine(format, args);
        }

        public void EndWrite()
        {
            process.StandardInput.Close();
        }
    }
}
