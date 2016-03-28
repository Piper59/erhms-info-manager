using Epi.Windows.Analysis.Forms;
using System;
using System.ComponentModel;
using System.Data;

namespace ERHMS.EpiInfo.Analysis
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
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            ProgramEditor.AddCommand(command);
        }

        public void ExecuteCommand(string command, Action callback = null)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                BeginInvoke(new Action(() =>
                {
                    ProgramEditor.txtTextArea.Enabled = false;
                    ProgramEditor.btnRun.Enabled = false;
                    ProgramEditor.ShowErrorMessage("");
                    UpdateStatus("Running PGM...", false);
                }));
                try
                {
                    EpiInterpreter.Context.ClearState();
                    if (string.IsNullOrEmpty(command))
                    {
                        return;
                    }
                    EpiInterpreter.Execute(command);
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        ProgramEditor.ShowErrorMessage(ex.ToString());
                    }));
                }
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                ProgramEditor.txtTextArea.Enabled = true;
                ProgramEditor.btnRun.Enabled = true;
                UpdateStatus("Ready", false);
                if (callback != null)
                {
                    callback();
                }
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
    }
}
