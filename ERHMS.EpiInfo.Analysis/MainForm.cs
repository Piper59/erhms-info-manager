using Epi.Windows.Analysis.Forms;
using System;
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

        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            EpiInterpreter.Execute(command);
        }

        public void AddAndExecuteCommand(string command)
        {
            AddCommand(command);
            ExecuteCommand(command);
        }

        public void ExecuteCommands()
        {
            ExecuteCommand(Commands);
        }

        public DataTable GetCurrentReadOutput()
        {
            EpiInterpreter.Context.GetOutput();
            return EpiInterpreter.Context.DataSet.Tables["Output"];
        }
    }
}
