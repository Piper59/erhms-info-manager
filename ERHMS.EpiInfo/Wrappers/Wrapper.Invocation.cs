using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ERHMS.EpiInfo.Wrappers
{
    public partial class Wrapper
    {
        protected static Wrapper Create(Expression<Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            string methodName = string.Format("{0}.{1}", body.Method.DeclaringType.FullName, body.Method.Name);
            IEnumerable<object> args = body.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke());
            return new Wrapper(Assembly.GetCallingAssembly(), methodName, args);
        }

        private ICollection<object> args;
        private Process process;
        private ManualResetEvent exited;

        public WaitHandle Exited
        {
            get { return exited; }
        }

        protected Wrapper()
        {
            throw new NotSupportedException();
        }

        private Wrapper(Assembly assembly, string methodName, IEnumerable<object> args)
        {
            string fileName = string.Format("{0}.exe", assembly.GetName().Name);
            string path = Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), fileName);
            this.args = args.ToList();
            process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    FileName = path,
                    Arguments = methodName
                }
            };
            exited = new ManualResetEvent(false);
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    OnEvent(e.Data);
                }
            };
            process.Exited += (sender, e) =>
            {
                Log.Logger.DebugFormat("Wrapper {0} exited", process.Id);
                exited.Set();
            };
        }

        public event EventHandler<WrapperEventArgs> Event;
        private void OnEvent(WrapperEventArgs e)
        {
            Log.Logger.DebugFormat("Event raised by wrapper {0}: {1}", process.Id, e.Type);
            Event?.Invoke(this, e);
        }
        private void OnEvent(string data)
        {
            OnEvent(new WrapperEventArgs(data));
        }

        public void Invoke()
        {
            process.Start();
            Log.Logger.DebugFormat("Invoked wrapper {0}: {1} {2}", process.Id, process.StartInfo.FileName, process.StartInfo.Arguments);
            if (args != null)
            {
                SendArgs();
            }
            process.BeginErrorReadLine();
        }

        private void SendArgs()
        {
            Log.Logger.DebugFormat("Sending args to wrapper {0}", process.Id);
            WriteLine(args.Count);
            foreach (object arg in args)
            {
                WriteLine(arg == null ? "" : ConvertExtensions.ToBase64String(arg));
            }
        }

        public string ReadLine()
        {
            return process.StandardOutput.ReadLine();
        }

        public string ReadToEnd()
        {
            return process.StandardOutput.ReadToEnd();
        }

        public void WriteLine()
        {
            process.StandardInput.WriteLine();
        }

        public void WriteLine(string value)
        {
            process.StandardInput.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args)
        {
            process.StandardInput.WriteLine(format, args);
        }

        public void WriteLine(object value)
        {
            process.StandardInput.WriteLine(value);
        }

        public void Close()
        {
            process.StandardInput.Close();
        }
    }
}
