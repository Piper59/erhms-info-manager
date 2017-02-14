using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ERHMS.EpiInfo.Wrappers
{
    public partial class Wrapper
    {
        protected static Wrapper Create(string methodName, WrapperArgsBase args = null)
        {
            return new Wrapper(Assembly.GetCallingAssembly(), methodName, args);
        }

        private string executablePath;
        private string methodName;
        private WrapperArgsBase args;
        private Process process;

        public ManualResetEvent Exited { get; private set; }

        protected Wrapper()
        {
            throw new NotSupportedException();
        }

        private Wrapper(Assembly assembly, string methodName, WrapperArgsBase args)
        {
            string fileName = string.Format("{0}.exe", assembly.GetName().Name);
            executablePath = Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), fileName);
            this.methodName = methodName;
            this.args = args;
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
            Log.Logger.DebugFormat("Invoking wrapper: {0} {1}", executablePath, methodName);
            process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(executablePath),
                    FileName = executablePath,
                    Arguments = methodName
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Log.Logger.DebugFormat("Event raised by wrapper {0}: {1}", process.Id, e.Data);
                    OnEvent(e.Data);
                }
            };
            process.Exited += (sender, e) =>
            {
                Log.Logger.DebugFormat("Wrapper {0} exited", process.Id);
                Exited.Set();
            };
            process.Start();
            Log.Logger.DebugFormat("Wrapper {0} invoked", process.Id);
            if (args != null)
            {
                Log.Logger.DebugFormat("Sending args to wrapper {0}: {1}", process.Id, args);
                WriteLine(args);
            }
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

        public void Close()
        {
            process.StandardInput.Close();
        }
    }
}
