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
            return new Wrapper(body.Method, body.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()));
        }

        private Process process;
        private ICollection<object> args;
        private EventWaitHandle closed;
        private EventWaitHandle exited;

        public WaitHandle Exited
        {
            get { return exited; }
        }

        protected Wrapper()
        {
            throw new NotSupportedException();
        }

        private Wrapper(MethodInfo method, IEnumerable<object> args)
        {
            process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(method.DeclaringType.Assembly.Location),
                    FileName = method.DeclaringType.Assembly.Location,
                    Arguments = string.Format("{0} {1}", method.DeclaringType.FullName, method.Name)
                }
            };
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;
            this.args = args.ToList();
            closed = new ManualResetEvent(false);
            exited = new ManualResetEvent(false);
        }

        public event EventHandler Invoked;
        private void OnInvoked(EventArgs e)
        {
            Invoked?.Invoke(this, e);
        }
        private void OnInvoked()
        {
            OnInvoked(EventArgs.Empty);
        }

        public event EventHandler<WrapperEventArgs> Event;
        private void OnEvent(WrapperEventArgs e)
        {
            Log.Logger.DebugFormat("Event raised by wrapper {0}: {1}", process.Id, e.Type);
            Event?.Invoke(this, e);
        }
        private void OnEvent(string data)
        {
            OnEvent(WrapperEventArgs.Deserialize(data));
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                closed.Set();
            }
            else
            {
                OnEvent(e.Data);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Log.Logger.DebugFormat("Wrapper {0} exited", process.Id);
            closed.WaitOne();
            exited.Set();
        }

        public void Invoke()
        {
            process.Start();
            Log.Logger.DebugFormat("Wrapper {0} invoked: {1} {2}", process.Id, process.StartInfo.FileName, process.StartInfo.Arguments);
            SendArgs();
            OnInvoked();
            process.BeginErrorReadLine();
        }

        private void SendArgs()
        {
            Log.Logger.DebugFormat("Sending args to wrapper {0}", process.Id);
            WriteLine(args.Count);
            foreach (object arg in args)
            {
                WriteLine(ConvertExtensions.ToBase64String(arg));
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
