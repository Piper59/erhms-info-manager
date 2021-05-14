using ERHMS.Common;
using ERHMS.Common.Linq;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.Desktop.Utilities
{
    public static partial class Utility
    {
        public static string Invoke(IUtility utility)
        {
            string result = null;
            try
            {
                using (Process process = GetProcess(utility))
                using (UtilityDialog dialog = GetDialog(utility))
                using (BackgroundWorker worker = new BackgroundWorker())
                {
                    worker.DoWork += (sender, e) =>
                    {
                        process.Start();
                        result = process.StandardOutput.ReadToEnd();
                    };
                    worker.RunWorkerCompleted += (sender, e) =>
                    {
                        if (e.Error != null)
                        {
                            OnError(e.Error);
                        }
                        dialog.Done = true;
                        dialog.Close();
                    };
                    dialog.Shown += (sender, e) =>
                    {
                        worker.RunWorkerAsync();
                    };
                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            return result;
        }

        private static Process GetProcess(IUtility utility)
        {
            string workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            IEnumerable<string> args = utility.Parameters.Prepend(utility.GetType().Name);
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectoryPath,
                    FileName = Path.Combine(workingDirectoryPath, "ERHMS Info Manager.exe"),
                    Arguments = CommandLine.Quote(args),
                    RedirectStandardOutput = true
                }
            };
        }

        private static UtilityDialog GetDialog(IUtility utility)
        {
            return new UtilityDialog
            {
                Owner = Form.ActiveForm,
                StartPosition = FormStartPosition.CenterParent,
                Body = utility.Instructions
            };
        }

        private static void OnError(Exception exception)
        {
            Log.Instance.Error(exception);
            MessageBox.Show(
                string.Format(ResXResources.Body_UtilityError, exception.Message),
                ResXResources.Title_App,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
