using ERHMS.Common;
using ERHMS.Desktop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERHMS.Desktop.Utilities
{
    public static class Utility
    {
        private const string AppAssemblyName = "ERHMS Info Manager";

        private static readonly IReadOnlyCollection<Type> instanceTypes = typeof(IUtility).Assembly.GetTypes()
            .Where(type => typeof(IUtility).IsAssignableFrom(type) && !type.IsAbstract)
            .ToList();

        private static void OnException(Exception exception)
        {
            Log.Instance.Error(exception);
            MessageBox.Show(
                string.Format(ResXResources.Body_IntegrationException, exception.Message),
                ResXResources.Title_App,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static Process GetProcess(IUtility utility)
        {
            string workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            List<string> args = new List<string>
            {
                utility.GetType().Name
            };
            args.AddRange(utility.GetParameters());
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectoryPath,
                    FileName = Path.Combine(workingDirectoryPath, $"{AppAssemblyName}.exe"),
                    Arguments = CommandLine.GetArguments(args),
                    RedirectStandardOutput = true
                }
            };
        }

        private static IntegrationForm GetForm(IUtility utility)
        {
            return new IntegrationForm
            {
                Owner = Form.ActiveForm,
                StartPosition = FormStartPosition.CenterParent,
                Body = utility.Instructions
            };
        }

        public static string Invoke(IUtility utility)
        {
            string result = null;
            try
            {
                Exception error = null;
                Process process = GetProcess(utility);
                IntegrationForm form = GetForm(utility);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    process.Start();
                    result = process.StandardOutput.ReadToEnd();
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    error = e.Error;
                    form.Done = true;
                    form.Close();
                };
                form.Shown += (sender, e) =>
                {
                    worker.RunWorkerAsync();
                };
                if (form.ShowDialog() == DialogResult.Cancel && !process.HasExited)
                {
                    process.Kill();
                }
                if (error != null)
                {
                    ExceptionDispatchInfo.Capture(error).Throw();
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
            return result;
        }

        public static async Task ExecuteAsync(IReadOnlyList<string> args)
        {
            string instanceTypeName = args[0];
            IReadOnlyList<string> parameters = args.Skip(1).ToList();
            Type instanceType = instanceTypes.SingleOrDefault(_instanceType => _instanceType.Name == instanceTypeName);
            if (instanceType == null)
            {
                throw new ArgumentException($"Utility '{instanceTypeName}' does not exist.");
            }
            Log.Instance.Debug($"Executing utility: {instanceTypeName}");
            IUtility utility = (IUtility)Activator.CreateInstance(instanceType);
            utility.ParseParameters(parameters);
            Console.Out.Write(await utility.ExecuteAsync());
            Console.Out.Close();
        }
    }
}
