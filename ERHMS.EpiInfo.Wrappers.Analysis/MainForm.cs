using Epi;
using Epi.Analysis.Dialogs;
using Epi.Windows.Analysis.Dialogs;
using Epi.Windows.Analysis.Forms;
using ERHMS.Utility;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Action = System.Action;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class MainForm : AnalysisMainForm
    {
        private WaitDialog waitDialog;

        public string Commands
        {
            get { return ProgramEditor.txtTextArea.Text; }
            set { ProgramEditor.txtTextArea.Text = value; }
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(AnalysisMainForm))
            {
                return this;
            }
            else
            {
                return base.GetService(serviceType);
            }
        }

        public MainForm()
        {
            WindowState = FormWindowState.Maximized;
            waitDialog = new WaitDialog();
        }

        private void BeginWait(bool showDialog)
        {
            ProgramEditor.ShowErrorMessage("");
            UpdateStatus("Running...", false);
            Enabled = false;
            if (showDialog)
            {
                waitDialog.Show(this);
            }
        }

        private void EndWait()
        {
            waitDialog.Hide();
            Enabled = true;
            UpdateStatus(SharedStrings.READY, false);
        }

        public void AddCommand(string command)
        {
            ProgramEditor.AddCommand(command);
        }

        public void ExecuteCommand(string command, bool showDialog, Action callback = null)
        {
            Log.Logger.DebugFormat("Executing command: {0}", command);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                EpiInterpreter.Context.ClearState();
                EpiInterpreter.Execute(command);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                EndWait();
                if (e.Error == null)
                {
                    callback?.Invoke();
                }
                else
                {
                    ProgramEditor.ShowErrorMessage(e.Error.ToString());
                    string message = "An error occurred while running commands. See the program editor's output window for details.";
                    MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            BeginWait(showDialog);
            worker.RunWorkerAsync();
        }

        public void ExecuteCommands(bool showDialog, Action callback = null)
        {
            ExecuteCommand(Commands, showDialog, callback);
        }

        public DataTable GetOutput()
        {
            EpiInterpreter.Context.GetOutput();
            return EpiInterpreter.Context.DataSet.Tables["Output"].Clone();
        }

        public bool SavePgm(string name)
        {
            using (PgmDialog dialog = new PgmDialog(this, name, Commands, PgmDialog.PgmDialogMode.SaveProgram))
            {
                dialog.StartPosition = FormStartPosition.CenterParent;
                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }
    }
}
