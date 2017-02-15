using Epi.Windows.Analysis.Dialogs;
using Epi.Windows.Analysis.Forms;
using ERHMS.Utility;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class MainForm : AnalysisMainForm
    {
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

        public void AddCommand(string command)
        {
            ProgramEditor.AddCommand(command);
        }

        public void ExecuteCommand(string command, Action callback = null)
        {
            Log.Logger.DebugFormat("Executing command: {0}", command);
            ProgramEditor.txtTextArea.Enabled = false;
            ProgramEditor.btnRun.Enabled = false;
            ProgramEditor.ShowErrorMessage("");
            UpdateStatus("Running...", false);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                EpiInterpreter.Context.ClearState();
                if (!string.IsNullOrWhiteSpace(command))
                {
                    EpiInterpreter.Execute(command);
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                ProgramEditor.txtTextArea.Enabled = true;
                ProgramEditor.btnRun.Enabled = true;
                if (e.Error != null)
                {
                    ProgramEditor.ShowErrorMessage(e.Error.ToString());
                }
                UpdateStatus("Ready", false);
                callback?.Invoke();
            };
            worker.RunWorkerAsync();
        }

        public void ExecuteCommands(Action callback = null)
        {
            ExecuteCommand(Commands, callback);
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
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }
    }
}
