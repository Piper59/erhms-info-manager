using ERHMS.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace ERHMS.EpiInfo
{
    public partial class Wrapper
    {
        protected static Wrapper Invoke(Expression<Action<string[]>> expression, params object[] args)
        {
            Wrapper wrapper = new Wrapper(Assembly.GetCallingAssembly(), expression, args);
            wrapper.Invoke();
            return wrapper;
        }

        private FileInfo executable;
        private string arguments;
        private Process process;
        private BlockingCollection<string> output;

        public ManualResetEvent Exited { get; private set; }

        protected Wrapper()
        {
            throw new NotSupportedException();
        }

        private Wrapper(Assembly assembly, Expression<Action<string[]>> expression, params object[] args)
        {
            string fileName = string.Format("{0}.exe", assembly.GetName().Name);
            executable = AssemblyExtensions.GetEntryDirectory().GetFile(fileName);
            string methodName = ((MethodCallExpression)expression.Body).Method.Name;
            arguments = ProcessExtensions.FormatArgs(args.Prepend(methodName));
            Exited = new ManualResetEvent(false);
        }

        protected Wrapper(Expression<Action<string[]>> expression, params object[] args)
            : this(Assembly.GetCallingAssembly(), expression, args) { }

        // TODO: Define events

        private void Invoke()
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
            output = new BlockingCollection<string>();
            process.OutputDataReceived += (sender, e) =>
            {
                // TODO: Translate output into events
                output.Add(e.Data);
            };
            process.Exited += (sender, e) =>
            {
                output.CompleteAdding();
                Exited.Set();
            };
            process.Start();
            process.BeginOutputReadLine();
        }

        public string ReadLine()
        {
            try
            {
                return output.Take();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public IEnumerable<string> ReadAllLines()
        {
            while (true)
            {
                string line = ReadLine();
                if (line == null)
                {
                    yield break;
                }
                else
                {
                    yield return line;
                }
            }
        }

        public string ReadToEnd()
        {
            return string.Join(Environment.NewLine, ReadAllLines());
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
