using Epi;
using Epi.Windows.Analysis.Dialogs;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Wrappers
{
    public class Analysis
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenPgm : Wrapper
        {
            private static string Content { get; set; }
            private static bool Execute { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string content, bool execute)
            {
                return Create(() => MainInternal(content, execute));
            }

            private static void MainInternal(string content, bool execute)
            {
                Content = content;
                Execute = execute;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                Form.Commands = Content;
                if (Execute)
                {
                    Form.ExecuteCommands(true);
                }
            }
        }

        public class Import : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static View View { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
            }

            private static void MainInternal(string projectPath, string viewName)
            {
                ProjectPath = projectPath;
                ViewName = viewName;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                BackgroundWorker worker = Form.GetBackgroundWorker("Loading...", true);
                worker.DoWork += Step1;
                worker.RunWorkerCompleted += Step2;
                worker.RunWorkerAsync();
            }

            private static void Step1(object sender, DoWorkEventArgs e)
            {
                Project project = new Project(ProjectPath);
                View = project.Views[ViewName];
            }

            private static void Step2(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Form.Panic("An error occurred while loading the project.", e.Error);
                    return;
                }
                string command;
                using (ReadDialog dialog = new ReadDialog(Form))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(Form) != DialogResult.OK)
                    {
                        Form.TryClose("No data source was selected. Import has been canceled.", MessageBoxIcon.Warning);
                        return;
                    }
                    command = dialog.CommandText;
                }
                Form.AddCommand(command);
                Form.ExecuteCommand(command, true, Step3);
            }

            private static void Step3(Exception ex)
            {
                if (ex != null)
                {
                    Form.Panic("An error occurred while reading data.", ex);
                    return;
                }
                ICollection<string> sources = Form.GetOutput().Columns.Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToList();
                ICollection<string> targets = View.Fields.DataFields.Cast<INamedObject>()
                    .Where(field => !(field is Epi.Fields.UniqueKeyField))
                    .Select(field => field.Name)
                    .ToList();
                MappingCollection mappings = null;
                while (true)
                {
                    using (MappingDialog dialog = new MappingDialog(Form, sources, targets))
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        if (dialog.ShowDialog(Form) == DialogResult.OK)
                        {
                            mappings = dialog.GetMappings();
                        }
                    }
                    if (mappings == null || mappings.Count == 0)
                    {
                        string message = "No variables were mapped. Retry mapping or cancel import?";
                        if (MessageBox.Show(Form, message, "Retry?", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                        {
                            Form.Close();
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(mappings.GetCommands());
                string path = IOExtensions.GetTempFileName("ERHMS_{0:N}.csv");
                builder.AppendLine(Commands.Write(path, mappings.GetTargets()));
                builder.AppendLine(Commands.Read(ProjectPath, ViewName));
                builder.AppendLine(Commands.Merge(path, mappings.GetIdTarget()));
                string command = builder.ToString().Trim();
                Form.AddCommand(command);
                Form.ExecuteCommand(command, true, Step4);
            }

            private static void Step4(Exception ex)
            {
                if (ex != null)
                {
                    Form.Panic("An error occurred while importing data.", ex);
                    return;
                }
                RaiseEvent("DataImported", new
                {
                    ViewId = View.Id
                });
                Form.TryClose("Data has been imported.");
            }
        }

        public class Export : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
            }

            private static void MainInternal(string projectPath, string viewName)
            {
                ProjectPath = projectPath;
                ViewName = viewName;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                string command = Commands.Read(ProjectPath, ViewName);
                Form.AddCommand(command);
                Form.ExecuteCommand(command, true, Step1);
            }

            private static void Step1(Exception ex)
            {
                if (ex != null)
                {
                    Form.Panic("An error occurred while reading data.", ex);
                    return;
                }
                using (WriteDialog dialog = new WriteDialog(Form))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(Form) == DialogResult.OK)
                    {
                        Form.AddCommand(dialog.CommandText);
                        if (dialog.ProcessingMode == CommandDesignDialog.CommandProcessingMode.Save_And_Execute)
                        {
                            Form.ExecuteCommand(dialog.CommandText, false, Step2);
                        }
                    }
                    else
                    {
                        Form.TryClose("Export has been canceled.", MessageBoxIcon.Warning);
                    }
                }
            }

            private static void Step2(Exception ex)
            {
                if (ex != null)
                {
                    Form.Panic("An error occurred while exporting data.", ex);
                    return;
                }
                Form.TryClose("Data has been exported.");
            }
        }
    }
}
