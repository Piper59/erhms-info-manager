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
            this.Initialize();
            waitDialog = new WaitDialog();
        }

        public BackgroundWorker GetBackgroundWorker(string message, bool showDialog)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                Invoke(new Action(() =>
                {
                    UpdateStatus(message, false);
                    Enabled = false;
                    if (showDialog)
                    {
                        waitDialog.Prompt = message;
                        waitDialog.Show(this);
                    }
                }));
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                waitDialog.Hide();
                Enabled = true;
                UpdateStatus(SharedStrings.READY, false);
            };
            return worker;
        }

        public void AddCommand(string command)
        {
            ProgramEditor.AddCommand(command);
        }

        public void ExecuteCommand(string command, bool showDialog, Action<Exception> callback)
        {
            Log.Logger.DebugFormat("Executing command: {0}", command);
            BackgroundWorker worker = GetBackgroundWorker("Running...", showDialog);
            worker.DoWork += (sender, e) =>
            {
                EpiInterpreter.Context.ClearState();
                EpiInterpreter.Execute(command);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    ProgramEditor.ShowErrorMessage(e.Error.ToString());
                }
                callback?.Invoke(e.Error);
            };
            ProgramEditor.ShowErrorMessage("");
            worker.RunWorkerAsync();
        }

        public void ExecuteCommands(bool showDialog, Action<Exception> callback = null)
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
